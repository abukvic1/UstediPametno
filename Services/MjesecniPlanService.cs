using UstediPametno.Models;
using UstediPametno.Repositories;

namespace UstediPametno.Services
{
    public class MjesecniPlanService : IMjesecniPlanService
    {
        private readonly IMjesecniPlanCiljService _planCiljService;
        private readonly IGenericRepository<MjesecniPlan> _repository;
        private readonly IGenericRepository<Transakcija> _transakcijaRepository;
        private readonly IGenericRepository<MjesecniPlanCilj> _planCiljRepository;
        private readonly ICiljStednjeService _ciljService;
        public MjesecniPlanService(
          IGenericRepository<MjesecniPlan> repository,
          IGenericRepository<Transakcija> transakcijaRepository,
          IGenericRepository<MjesecniPlanCilj> planCiljRepository,
          ICiljStednjeService ciljService,
          IMjesecniPlanCiljService planCiljService)
        {
            _repository = repository;
            _transakcijaRepository = transakcijaRepository;
            _planCiljRepository = planCiljRepository;
            _ciljService = ciljService;
            _planCiljService = planCiljService;
        }
        public async Task<IEnumerable<MjesecniPlan>> GetAllAsync(
            string korisnikId)
        {
            var planovi =
                await _repository.FindAsync(
                    plan => plan.KorisnikId == korisnikId);

            return planovi
                .OrderByDescending(plan => plan.Godina)
                .ThenByDescending(plan => plan.Mjesec);
        }

        public async Task<MjesecniPlan?> GetByIdAsync(
            int id,
            string korisnikId)
        {
            var planovi =
                await _repository.FindAsync(
                    plan =>
                        plan.Id == id &&
                        plan.KorisnikId == korisnikId);

            return planovi.FirstOrDefault();
        }

        public async Task<MjesecniPlan?> GetByPeriodAsync(
            int godina,
            int mjesec,
            string korisnikId)
        {
            var planovi =
                await _repository.FindAsync(
                    plan =>
                        plan.Godina == godina &&
                        plan.Mjesec == mjesec &&
                        plan.KorisnikId == korisnikId);

            return planovi.FirstOrDefault();
        }
        public async Task<MjesecniPlan> KreirajAsync(
    int godina,
    int mjesec,
    string korisnikId)
        {
            ValidirajPeriod(
                godina,
                mjesec);

            var postojeci =
                await GetByPeriodAsync(
                    godina,
                    mjesec,
                    korisnikId);

            if (postojeci != null)
            {
                throw new InvalidOperationException(
                    "Mjesečni plan za odabrani period već postoji.");
            }
            DateTime trenutniPeriod =
    new DateTime(godina, mjesec, 1);

            DateTime prethodniPeriod =
                trenutniPeriod.AddMonths(-1);

            MjesecniPlan? prethodniPlan =
                await GetByPeriodAsync(
                    prethodniPeriod.Year,
                    prethodniPeriod.Month,
                    korisnikId);

            decimal preneseno =
                prethodniPlan?.RaspolozivoZaTrosenje ?? 0;

            MjesecniPlan noviPlan =
     new MjesecniPlan
     {
         KorisnikId = korisnikId,

         Godina = godina,
         Mjesec = mjesec,

         UkupanPrihod = 0,

         PrenesenoIzPrethodnogMjeseca =
             Math.Round(preneseno, 2),

         UkupniFiksniTroskovi = 0,

         PotrebnaStednja = 0,

         StvarnoPotroseno = 0,

         StvarnoUstedjeno = 0,

         RaspolozivoZaTrosenje = 0,

         DnevniBudzet = 0
     };
            await _repository.AddAsync(
                noviPlan);

            await _repository.SaveChangesAsync();

            await PoveziPostojeceTransakcijeAsync(
                noviPlan,
                korisnikId);

            return noviPlan;
        }
        public async Task PropagirajStanjeOdMjesecaAsync(
    int godina,
    int mjesec,
    string korisnikId)
        {
            IEnumerable<MjesecniPlan> sviPlanovi =
                await GetAllAsync(korisnikId);

            List<MjesecniPlan> planovi =
                sviPlanovi
                    .OrderBy(plan => plan.Godina)
                    .ThenBy(plan => plan.Mjesec)
                    .ToList();

            DateTime pocetniPeriod =
                new DateTime(godina, mjesec, 1);

            decimal preneseno = 0;

            for (int i = 0; i < planovi.Count; i++)
            {
                MjesecniPlan plan = planovi[i];

                DateTime periodPlana =
                    new DateTime(
                        plan.Godina,
                        plan.Mjesec,
                        1);

                if (periodPlana < pocetniPeriod)
                {
                    continue;
                }

                MjesecniPlan? prethodniPlan =
                    planovi
                        .Where(p =>
                            new DateTime(
                                p.Godina,
                                p.Mjesec,
                                1) < periodPlana)
                        .OrderByDescending(p => p.Godina)
                        .ThenByDescending(p => p.Mjesec)
                        .FirstOrDefault();

                if (prethodniPlan != null)
                {
                    preneseno =
                        prethodniPlan.RaspolozivoZaTrosenje;
                }
                else
                {
                    preneseno = 0;
                }

                plan.PrenesenoIzPrethodnogMjeseca =
                    Math.Round(preneseno, 2);

                _repository.Update(plan);
                await _repository.SaveChangesAsync();

                await PonovoIzracunajAsync(
                    plan.Id,
                    korisnikId);
            }
        }

        public async Task<bool> PonovoIzracunajAsync(
     int id,
     string korisnikId)
        {
            MjesecniPlan? plan =
                await GetByIdAsync(
                    id,
                    korisnikId);

            if (plan == null)
            {
                return false;
            }
            await _planCiljService.DodajAktivneCiljeveZaPlanAsync(
    plan.Id,
    plan.Godina,
    plan.Mjesec,
    korisnikId);
            var transakcije =
                await _transakcijaRepository.FindAsync(
                    transakcija =>
                        transakcija.KorisnikId == korisnikId &&
                        transakcija.Datum.Year == plan.Godina &&
                        transakcija.Datum.Month == plan.Mjesec);

            foreach (var transakcija in transakcije)
            {
                if (transakcija.MjesecniPlanId != plan.Id)
                {
                    transakcija.MjesecniPlanId = plan.Id;

                    _transakcijaRepository.Update(
                        transakcija);
                }
            }

            await _transakcijaRepository.SaveChangesAsync();

            decimal prihod =
                transakcije
                    .Where(transakcija =>
                        transakcija.Vrsta ==
                        VrstaTransakcije.Prihod)
                    .Sum(transakcija =>
                        transakcija.Iznos);

            decimal fiksniTroskovi =
                transakcije
                    .Where(transakcija =>
                        transakcija.Vrsta ==
                        VrstaTransakcije.FiksniTrosak)
                    .Sum(transakcija =>
                        transakcija.Iznos);

            decimal dnevnaPotrosnja =
                transakcije
                    .Where(transakcija =>
                        transakcija.Vrsta ==
                        VrstaTransakcije.DnevnaPotrosnja)
                    .Sum(transakcija =>
                        transakcija.Iznos);

            decimal ustedjeno =
                transakcije
                    .Where(transakcija =>
                        transakcija.Vrsta ==
                        VrstaTransakcije.UplataStednje)
                    .Sum(transakcija =>
                        transakcija.Iznos);

            decimal potroseno =
                fiksniTroskovi +
                dnevnaPotrosnja;

            IEnumerable<MjesecniPlanCilj> planiraniCiljevi =
                await _planCiljRepository.FindAsync(
                    veza =>
                        veza.MjesecniPlanId == plan.Id);

            decimal potrebnaStednja =
                planiraniCiljevi.Sum(
                    veza => veza.PlaniraniIznos);
            decimal preostaloZaCiljeve =
    Math.Max(
        0,
        potrebnaStednja - ustedjeno);
            decimal raspolozivo =
                plan.PrenesenoIzPrethodnogMjeseca
                + prihod
                - potroseno
                - ustedjeno;

            raspolozivo =
                Math.Max(
                    0,
                    raspolozivo);

            int preostaliDani =
                IzracunajPreostaleDane(
                    plan.Godina,
                    plan.Mjesec);

            decimal dnevniBudzet =
                preostaliDani > 0
                    ? raspolozivo / preostaliDani
                    : 0;

            plan.UkupanPrihod =
                Math.Round(
                    prihod,
                    2);

            plan.UkupniFiksniTroskovi =
                Math.Round(
                    fiksniTroskovi,
                    2);

            plan.PotrebnaStednja =
                Math.Round(
                    potrebnaStednja,
                    2);

            plan.StvarnoPotroseno =
                Math.Round(
                    potroseno,
                    2);

            plan.StvarnoUstedjeno =
                Math.Round(
                    ustedjeno,
                    2);

            plan.RaspolozivoZaTrosenje =
                Math.Round(
                    raspolozivo,
                    2);

            plan.DnevniBudzet =
                Math.Round(
                    dnevniBudzet,
                    2);

            _repository.Update(plan);

            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> AzurirajStvarneIznoseAsync(
            int id,
            decimal stvarnoPotroseno,
            decimal stvarnoUstedjeno,
            string korisnikId)
        {
            MjesecniPlan? plan =
                await GetByIdAsync(
                    id,
                    korisnikId);

            if (plan == null)
            {
                return false;
            }

            plan.StvarnoPotroseno =
                stvarnoPotroseno;

            plan.StvarnoUstedjeno =
                stvarnoUstedjeno;

            _repository.Update(
                plan);

            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(
            int id,
            string korisnikId)
        {
            MjesecniPlan? plan =
                await GetByIdAsync(
                    id,
                    korisnikId);

            if (plan == null)
            {
                return false;
            }

            var transakcije =
                await _transakcijaRepository.FindAsync(
                    transakcija =>
                        transakcija.MjesecniPlanId == id &&
                        transakcija.KorisnikId == korisnikId);

            foreach (var transakcija in transakcije)
            {
                transakcija.MjesecniPlanId =
                    null;

                _transakcijaRepository.Update(
                    transakcija);
            }

            await _transakcijaRepository
                .SaveChangesAsync();

            _repository.Remove(
                plan);

            await _repository.SaveChangesAsync();

            return true;
        }

        private async Task PoveziPostojeceTransakcijeAsync(
            MjesecniPlan plan,
            string korisnikId)
        {
            var transakcije =
                await _transakcijaRepository.FindAsync(
                    transakcija =>
                        transakcija.KorisnikId == korisnikId &&
                        transakcija.Datum.Year == plan.Godina &&
                        transakcija.Datum.Month == plan.Mjesec);

            foreach (var transakcija in transakcije)
            {
                transakcija.MjesecniPlanId =
                    plan.Id;

                _transakcijaRepository.Update(
                    transakcija);
            }

            await _transakcijaRepository
                .SaveChangesAsync();
        }

        private static decimal IzracunajPotrebnuStednju(
            CiljStednje? cilj,
            int godina,
            int mjesec)
        {
            if (cilj == null ||
                cilj.Status == StatusCilja.Ostvaren)
            {
                return 0;
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

            return Math.Round(
                preostalo / brojMjeseci,
                2);
        }

        private static int IzracunajPreostaleDane(
            int godina,
            int mjesec)
        {
            DateTime danas =
                DateTime.Today;

            DateTime pocetak =
                new DateTime(
                    godina,
                    mjesec,
                    1);

            DateTime kraj =
                new DateTime(
                    godina,
                    mjesec,
                    DateTime.DaysInMonth(
                        godina,
                        mjesec));

            if (danas < pocetak)
            {
                return DateTime.DaysInMonth(
                    godina,
                    mjesec);
            }

            if (danas > kraj)
            {
                return 0;
            }

            return
                DateTime.DaysInMonth(
                    godina,
                    mjesec)
                - danas.Day
                + 1;
        }
        public async Task<bool> AzurirajPlaniranuStednjuAsync(
    int id,
    decimal iznos,
    string korisnikId)
        {
            MjesecniPlan? plan =
                await GetByIdAsync(
                    id,
                    korisnikId);

            if (plan == null)
            {
                return false;
            }

            plan.PotrebnaStednja =
                Math.Round(iznos, 2);

            _repository.Update(plan);
            await _repository.SaveChangesAsync();

            return true;
        }
        private static void ValidirajPeriod(
            int godina,
            int mjesec)
        {
            if (godina < 2000 ||
                godina > 2100)
            {
                throw new ArgumentException(
                    "Godina nije ispravna.");
            }

            if (mjesec < 1 ||
                mjesec > 12)
            {
                throw new ArgumentException(
                    "Mjesec mora biti između 1 i 12.");
            }
        }
    }
}