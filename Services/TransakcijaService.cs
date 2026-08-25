using UstediPametno.Models;
using UstediPametno.Repositories;

namespace UstediPametno.Services
{
    public class TransakcijaService : ITransakcijaService
    {
        private readonly IGenericRepository<Transakcija> _repository;
        private readonly IMjesecniPlanService _planService;
        private readonly ICiljStednjeService _ciljService;

        public TransakcijaService(
            IGenericRepository<Transakcija> repository,
            IMjesecniPlanService planService,
            ICiljStednjeService ciljService)
        {
            _repository = repository;
            _planService = planService;
            _ciljService = ciljService;
        }

        public async Task<IEnumerable<Transakcija>> GetAllAsync(
            string korisnikId)
        {
            IEnumerable<Transakcija> transakcije =
                await _repository.FindAsync(
                    transakcija =>
                        transakcija.KorisnikId == korisnikId);

            return transakcije
                .OrderByDescending(transakcija => transakcija.Datum);
        }

        public async Task<IEnumerable<Transakcija>> GetByPlanAsync(
            int mjesecniPlanId,
            string korisnikId)
        {
            IEnumerable<Transakcija> transakcije =
                await _repository.FindAsync(
                    transakcija =>
                        transakcija.MjesecniPlanId ==
                            mjesecniPlanId &&
                        transakcija.KorisnikId == korisnikId);

            return transakcije
                .OrderByDescending(transakcija => transakcija.Datum);
        }

        public async Task<Transakcija?> GetByIdAsync(
            int id,
            string korisnikId)
        {
            IEnumerable<Transakcija> transakcije =
                await _repository.FindAsync(
                    transakcija =>
                        transakcija.Id == id &&
                        transakcija.KorisnikId == korisnikId);

            return transakcije.FirstOrDefault();
        }

        public async Task AddAsync(
            Transakcija transakcija,
            string korisnikId)
        {
            PripremiDatum(transakcija);
            await PoveziSaPlanomPremaDatumuAsync(
    transakcija,
    korisnikId);
            ValidirajOsnovnePodatke(transakcija);

            await ValidirajVezeAsync(
                transakcija,
                korisnikId);

            if (transakcija.Vrsta ==
                VrstaTransakcije.UplataStednje)
            {
                await DodajNaCiljAsync(
                    transakcija,
                    korisnikId);
            }

            transakcija.Opis = transakcija.Opis.Trim();
            transakcija.KorisnikId = korisnikId;

            await _repository.AddAsync(transakcija);
            await _repository.SaveChangesAsync();

            await AzurirajPlanAsync(
                transakcija.MjesecniPlanId,
                korisnikId);
        }

        public async Task<bool> UpdateAsync(
            Transakcija transakcija,
            string korisnikId)
        {
            PripremiDatum(transakcija);
            await PoveziSaPlanomPremaDatumuAsync(
    transakcija,
    korisnikId);
            ValidirajOsnovnePodatke(transakcija);

            Transakcija? postojeca =
                await GetByIdAsync(
                    transakcija.Id,
                    korisnikId);

            if (postojeca == null)
            {
                return false;
            }

            await ValidirajVezeAsync(
                transakcija,
                korisnikId);

            int? stariPlanId =
                postojeca.MjesecniPlanId;

            if (postojeca.Vrsta ==
                    VrstaTransakcije.UplataStednje &&
                postojeca.CiljStednjeId.HasValue)
            {
                bool oduzeto =
                    await _ciljService.OduzmiUplatuAsync(
                        postojeca.CiljStednjeId.Value,
                        postojeca.Iznos,
                        korisnikId);

                if (!oduzeto)
                {
                    throw new InvalidOperationException(
                        "Prethodni cilj štednje ne postoji.");
                }
            }

            if (transakcija.Vrsta ==
                VrstaTransakcije.UplataStednje)
            {
                await DodajNaCiljAsync(
                    transakcija,
                    korisnikId);
            }

            postojeca.MjesecniPlanId =
                transakcija.MjesecniPlanId;

            postojeca.CiljStednjeId =
                transakcija.CiljStednjeId;

            postojeca.Vrsta =
                transakcija.Vrsta;

            postojeca.Iznos =
                transakcija.Iznos;

            postojeca.Opis =
                transakcija.Opis.Trim();

            postojeca.Datum =
                transakcija.Datum;

            _repository.Update(postojeca);
            await _repository.SaveChangesAsync();

            await AzurirajPlanAsync(
                stariPlanId,
                korisnikId);

            if (stariPlanId !=
                transakcija.MjesecniPlanId)
            {
                await AzurirajPlanAsync(
                    transakcija.MjesecniPlanId,
                    korisnikId);
            }

            return true;
        }

        public async Task<bool> DeleteAsync(
            int id,
            string korisnikId)
        {
            Transakcija? postojeca =
                await GetByIdAsync(id, korisnikId);

            if (postojeca == null)
            {
                return false;
            }

            int? planId =
                postojeca.MjesecniPlanId;

            if (postojeca.Vrsta ==
                    VrstaTransakcije.UplataStednje &&
                postojeca.CiljStednjeId.HasValue)
            {
                bool oduzeto =
                    await _ciljService.OduzmiUplatuAsync(
                        postojeca.CiljStednjeId.Value,
                        postojeca.Iznos,
                        korisnikId);

                if (!oduzeto)
                {
                    throw new InvalidOperationException(
                        "Cilj štednje ne postoji.");
                }
            }

            _repository.Remove(postojeca);
            await _repository.SaveChangesAsync();

            await AzurirajPlanAsync(
                planId,
                korisnikId);

            return true;
        }

        public async Task<decimal> GetUkupnoPotrosenoZaPlanAsync(
            int mjesecniPlanId,
            string korisnikId)
        {
            IEnumerable<Transakcija> transakcije =
                await GetByPlanAsync(
                    mjesecniPlanId,
                    korisnikId);

            return transakcije
                .Where(transakcija =>
                    transakcija.Vrsta ==
                        VrstaTransakcije.FiksniTrosak ||
                    transakcija.Vrsta ==
                        VrstaTransakcije.DnevnaPotrosnja)
                .Sum(transakcija => transakcija.Iznos);
        }

        public async Task<decimal> GetUkupnoUstedjenoZaPlanAsync(
            int mjesecniPlanId,
            string korisnikId)
        {
            IEnumerable<Transakcija> transakcije =
                await GetByPlanAsync(
                    mjesecniPlanId,
                    korisnikId);

            return transakcije
                .Where(transakcija =>
                    transakcija.Vrsta ==
                        VrstaTransakcije.UplataStednje)
                .Sum(transakcija => transakcija.Iznos);
        }

        private async Task ValidirajVezeAsync(
            Transakcija transakcija,
            string korisnikId)
        {
            if (transakcija.MjesecniPlanId.HasValue)
            {
                MjesecniPlan? plan =
                    await _planService.GetByIdAsync(
                        transakcija.MjesecniPlanId.Value,
                        korisnikId);

                if (plan == null)
                {
                    throw new ArgumentException(
                        "Odabrani mjesečni plan ne postoji.");
                }

                if (transakcija.Datum.Year != plan.Godina ||
                    transakcija.Datum.Month != plan.Mjesec)
                {
                    throw new ArgumentException(
                        "Datum transakcije mora pripadati " +
                        "odabranom mjesečnom planu.");
                }
            }

            if (transakcija.CiljStednjeId.HasValue)
            {
                CiljStednje? cilj =
                    await _ciljService.GetByIdAsync(
                        transakcija.CiljStednjeId.Value,
                        korisnikId);

                if (cilj == null)
                {
                    throw new ArgumentException(
                        "Odabrani cilj štednje ne postoji.");
                }
            }

            if (transakcija.Vrsta ==
                    VrstaTransakcije.UplataStednje &&
                !transakcija.CiljStednjeId.HasValue)
            {
                throw new ArgumentException(
                    "Za uplatu štednje moraš odabrati cilj.");
            }

            if (transakcija.Vrsta !=
                    VrstaTransakcije.UplataStednje &&
                transakcija.CiljStednjeId.HasValue)
            {
                throw new ArgumentException(
                    "Cilj se može odabrati samo za uplatu štednje.");
            }
        }

        private async Task DodajNaCiljAsync(
            Transakcija transakcija,
            string korisnikId)
        {
            if (!transakcija.CiljStednjeId.HasValue)
            {
                throw new ArgumentException(
                    "Cilj štednje nije odabran.");
            }

            bool dodano =
                await _ciljService.DodajUplatuAsync(
                    transakcija.CiljStednjeId.Value,
                    transakcija.Iznos,
                    korisnikId);

            if (!dodano)
            {
                throw new InvalidOperationException(
                    "Cilj štednje ne postoji.");
            }
        }

        private async Task AzurirajPlanAsync(
     int? planId,
     string korisnikId)
        {
            if (!planId.HasValue)
            {
                return;
            }

            MjesecniPlan? plan =
                await _planService.GetByIdAsync(
                    planId.Value,
                    korisnikId);

            if (plan == null)
            {
                return;
            }

            await _planService.PonovoIzracunajAsync(
                plan.Id,
                korisnikId);

            await _planService.PropagirajStanjeOdMjesecaAsync(
                plan.Godina,
                plan.Mjesec,
                korisnikId);
        }
        public async Task<decimal> GetUkupanPrihodAsync(
    string korisnikId)
        {
            var transakcije =
                await GetAllAsync(korisnikId);

            return transakcije
                .Where(t =>
                    t.Vrsta ==
                    VrstaTransakcije.Prihod)
                .Sum(t => t.Iznos);
        }
        private static void PripremiDatum(
            Transakcija transakcija)
        {
            if (transakcija.Datum == default)
            {
                transakcija.Datum = DateTime.Now;
            }
        }

        private static void ValidirajOsnovnePodatke(
            Transakcija transakcija)
        {
            if (transakcija.Iznos <= 0)
            {
                throw new ArgumentException(
                    "Iznos transakcije mora biti veći od nule.");
            }

            if (string.IsNullOrWhiteSpace(transakcija.Opis))
            {
                throw new ArgumentException(
                    "Opis transakcije je obavezan.");
            }

            if (!Enum.IsDefined(
                typeof(VrstaTransakcije),
                transakcija.Vrsta))
            {
                throw new ArgumentException(
                    "Vrsta transakcije nije ispravna.");
            }
        }
        private async Task PoveziSaPlanomPremaDatumuAsync(
    Transakcija transakcija,
    string korisnikId)
        {
            MjesecniPlan? plan =
                await _planService.GetByPeriodAsync(
                    transakcija.Datum.Year,
                    transakcija.Datum.Month,
                    korisnikId);

            transakcija.MjesecniPlanId = plan?.Id;
        }
        public async Task<decimal> GetUkupneRashodeAsync(
    string korisnikId)
        {
            var transakcije =
                await GetAllAsync(korisnikId);

            return transakcije
                .Where(t =>
                    t.Vrsta == VrstaTransakcije.FiksniTrosak ||
                    t.Vrsta == VrstaTransakcije.DnevnaPotrosnja)
                .Sum(t => t.Iznos);
        }
        public async Task<decimal> GetUkupnoUstedjenoAsync(
    string korisnikId)
        {
            var transakcije =
                await GetAllAsync(korisnikId);

            return transakcije
                .Where(t =>
                    t.Vrsta == VrstaTransakcije.UplataStednje)
                .Sum(t => t.Iznos);
        }
    }

}