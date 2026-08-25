using UstediPametno.Models;
using UstediPametno.Repositories;

namespace UstediPametno.Services
{
    public class MjesecniPlanCiljService
        : IMjesecniPlanCiljService
    {
        private readonly IGenericRepository<MjesecniPlanCilj> _repository;
        private readonly ICiljStednjeService _ciljService;

        public MjesecniPlanCiljService(
            IGenericRepository<MjesecniPlanCilj> repository,
            ICiljStednjeService ciljService)
        {
            _repository = repository;
            _ciljService = ciljService;
        }
        public async Task SinhronizujCiljSaPlanovimaAsync(
    int ciljId,
    string korisnikId)
        {
            CiljStednje? cilj =
                await _ciljService.GetByIdAsync(
                    ciljId,
                    korisnikId);

            if (cilj == null)
            {
                return;
            }

            IEnumerable<MjesecniPlanCilj> sveVeze =
                await _repository.FindAsync(
                    veza => veza.CiljStednjeId == ciljId);

            foreach (MjesecniPlanCilj veza in sveVeze)
            {
                MjesecniPlan plan = veza.MjesecniPlan;

                DateTime pocetakMjeseca =
                    new DateTime(
                        plan.Godina,
                        plan.Mjesec,
                        1);

                DateTime krajMjeseca =
                    new DateTime(
                        plan.Godina,
                        plan.Mjesec,
                        DateTime.DaysInMonth(
                            plan.Godina,
                            plan.Mjesec));

                bool ciljVazi =
                    cilj.DatumPocetka.Date <= krajMjeseca.Date &&
                    cilj.DatumZavrsetka.Date >= pocetakMjeseca.Date;

                if (!ciljVazi ||
                    cilj.Status == StatusCilja.Ostvaren)
                {
                    _repository.Remove(veza);
                }
            }

            await _repository.SaveChangesAsync();
        }
        public async Task ObrisiVezuAsync(
    int mjesecniPlanId,
    int ciljId)
        {
            IEnumerable<MjesecniPlanCilj> veze =
                await _repository.FindAsync(
                    veza =>
                        veza.MjesecniPlanId == mjesecniPlanId &&
                        veza.CiljStednjeId == ciljId);

            MjesecniPlanCilj? veza =
                veze.FirstOrDefault();

            if (veza == null)
            {
                return;
            }

            _repository.Remove(veza);
            await _repository.SaveChangesAsync();
        }
        public async Task DodajCiljeveAsync(
            int mjesecniPlanId,
            IEnumerable<int> ciljIds,
            int godina,
            int mjesec,
            string korisnikId)
        {
            foreach (int ciljId in ciljIds.Distinct())
            {
                CiljStednje? cilj =
                    await _ciljService.GetByIdAsync(
                        ciljId,
                        korisnikId);

                if (cilj == null)
                {
                    throw new ArgumentException(
                        "Jedan od odabranih ciljeva ne postoji.");
                }

                if (cilj.Status == StatusCilja.Ostvaren)
                {
                    continue;
                }

                decimal preostalo =
                    Math.Max(
                        0,
                        cilj.CiljaniIznos -
                        cilj.TrenutnoUstedjeno);

                int brojMjeseci =
                    (cilj.DatumZavrsetka.Year - godina) * 12 +
                    cilj.DatumZavrsetka.Month -
                    mjesec +
                    1;

                if (brojMjeseci < 1)
                {
                    brojMjeseci = 1;
                }

                decimal planiraniIznos =
                    Math.Round(
                        preostalo / brojMjeseci,
                        2);

                MjesecniPlanCilj veza =
                    new MjesecniPlanCilj
                    {
                        MjesecniPlanId = mjesecniPlanId,
                        CiljStednjeId = ciljId,
                        PlaniraniIznos = planiraniIznos
                    };

                await _repository.AddAsync(veza);
            }

            await _repository.SaveChangesAsync();
        }
        public async Task DodajAktivneCiljeveZaPlanAsync(
    int mjesecniPlanId,
    int godina,
    int mjesec,
    string korisnikId)
        {
            IEnumerable<CiljStednje> sviCiljevi =
                await _ciljService.GetAllAsync(korisnikId);

            DateTime pocetakMjeseca =
                new DateTime(godina, mjesec, 1);

            DateTime krajMjeseca =
                new DateTime(
                    godina,
                    mjesec,
                    DateTime.DaysInMonth(godina, mjesec));

            IEnumerable<MjesecniPlanCilj> postojeceVeze =
                await GetByPlanAsync(mjesecniPlanId);

            foreach (CiljStednje cilj in sviCiljevi)
            {
                if (cilj.Status == StatusCilja.Ostvaren)
                {
                    continue;
                }

                bool ciljVaziZaMjesec =
                    cilj.DatumPocetka.Date <= krajMjeseca.Date &&
                    cilj.DatumZavrsetka.Date >= pocetakMjeseca.Date;

                if (!ciljVaziZaMjesec)
                {
                    continue;
                }

                bool vecPostoji =
                    postojeceVeze.Any(
                        veza => veza.CiljStednjeId == cilj.Id);

                if (vecPostoji)
                {
                    continue;
                }

                decimal preostalo =
                    Math.Max(
                        0,
                        cilj.CiljaniIznos -
                        cilj.TrenutnoUstedjeno);

                int brojMjeseci =
                    (cilj.DatumZavrsetka.Year - godina) * 12 +
                    cilj.DatumZavrsetka.Month -
                    mjesec +
                    1;

                if (brojMjeseci < 1)
                {
                    brojMjeseci = 1;
                }

                decimal planiraniIznos =
                    Math.Round(
                        preostalo / brojMjeseci,
                        2);

                MjesecniPlanCilj veza =
                    new MjesecniPlanCilj
                    {
                        MjesecniPlanId = mjesecniPlanId,
                        CiljStednjeId = cilj.Id,
                        PlaniraniIznos = planiraniIznos
                    };

                await _repository.AddAsync(veza);
            }

            await _repository.SaveChangesAsync();
        }

        public async Task<IEnumerable<MjesecniPlanCilj>>
            GetByPlanAsync(int mjesecniPlanId)
        {
            return await _repository.FindAsync(
                x => x.MjesecniPlanId == mjesecniPlanId);
        }

        public async Task<decimal> GetUkupnoPlaniranoAsync(
            int mjesecniPlanId)
        {
            IEnumerable<MjesecniPlanCilj> ciljevi =
                await GetByPlanAsync(mjesecniPlanId);

            return ciljevi.Sum(
                x => x.PlaniraniIznos);
        }

    }
}