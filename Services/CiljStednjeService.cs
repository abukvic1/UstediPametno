using UstediPametno.Models;
using UstediPametno.Repositories;

namespace UstediPametno.Services
{
    public class CiljStednjeService : ICiljStednjeService
    {
        private readonly IGenericRepository<CiljStednje> _repository;
        private readonly IGenericRepository<Transakcija> _transakcijaRepository;
        private readonly IIzvorPrihodaService _izvorPrihodaService;
        private readonly IGenericRepository<MjesecniPlanCilj> _mjesecniPlanCiljRepository;
        public CiljStednjeService(
    IGenericRepository<CiljStednje> repository,
    IGenericRepository<Transakcija> transakcijaRepository,
    IGenericRepository<MjesecniPlanCilj> mjesecniPlanCiljRepository,
    IIzvorPrihodaService izvorPrihodaService)
        {
            _repository = repository;
            _transakcijaRepository = transakcijaRepository;
            _mjesecniPlanCiljRepository = mjesecniPlanCiljRepository;
            _izvorPrihodaService = izvorPrihodaService;
        }

        public async Task<IEnumerable<CiljStednje>> GetAllAsync(
            string korisnikId)
        {
            return await _repository.FindAsync(
                cilj => cilj.KorisnikId == korisnikId);
        }

        public async Task<IEnumerable<CiljStednje>> GetAktivneAsync(
            string korisnikId)
        {
            return await _repository.FindAsync(
                cilj =>
                    cilj.KorisnikId == korisnikId &&
                    (cilj.Status == StatusCilja.UToku ||
                     cilj.Status == StatusCilja.Prilagodjen));
        }

        public async Task<CiljStednje?> GetByIdAsync(
            int id,
            string korisnikId)
        {
            IEnumerable<CiljStednje> ciljevi =
                await _repository.FindAsync(
                    cilj =>
                        cilj.Id == id &&
                        cilj.KorisnikId == korisnikId);

            return ciljevi.FirstOrDefault();
        }

        public async Task AddAsync(
            CiljStednje ciljStednje,
            string korisnikId)
        {
            Validiraj(ciljStednje);

            ciljStednje.Naziv =
                ciljStednje.Naziv.Trim();

            ciljStednje.KorisnikId =
                korisnikId;

            ciljStednje.TrenutnoUstedjeno =
                0;

            ciljStednje.DatumPocetka =
                DateTime.UtcNow;

            ciljStednje.Status =
                StatusCilja.UToku;

            await _repository.AddAsync(ciljStednje);
            await _repository.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(
            CiljStednje ciljStednje,
            string korisnikId)
        {
            Validiraj(ciljStednje);

            CiljStednje? postojeci =
                await GetByIdAsync(
                    ciljStednje.Id,
                    korisnikId);

            if (postojeci == null)
            {
                return false;
            }

            postojeci.Naziv =
                ciljStednje.Naziv.Trim();

            postojeci.CiljaniIznos =
                ciljStednje.CiljaniIznos;

            postojeci.DatumZavrsetka =
                ciljStednje.DatumZavrsetka;

            AzurirajStatus(postojeci);

            _repository.Update(postojeci);
            await _repository.SaveChangesAsync();

            return true;
        }


        public async Task<bool> DeleteAsync(
    int id,
    string korisnikId)
        {
            CiljStednje? postojeci =
                await GetByIdAsync(id, korisnikId);

            if (postojeci == null)
            {
                return false;
            }

            IEnumerable<Transakcija> povezaneTransakcije =
                await _transakcijaRepository.FindAsync(
                    t =>
                        t.KorisnikId == korisnikId &&
                        t.CiljStednjeId == id);

            foreach (Transakcija transakcija in povezaneTransakcije)
            {
                transakcija.CiljStednjeId = null;
                _transakcijaRepository.Update(transakcija);
            }

            await _transakcijaRepository.SaveChangesAsync();

            IEnumerable<MjesecniPlanCilj> vezeSaPlanovima =
                await _mjesecniPlanCiljRepository.FindAsync(
                    x => x.CiljStednjeId == id);

            foreach (MjesecniPlanCilj veza in vezeSaPlanovima)
            {
                _mjesecniPlanCiljRepository.Remove(veza);
            }

            await _mjesecniPlanCiljRepository.SaveChangesAsync();

            _repository.Remove(postojeci);
            await _repository.SaveChangesAsync();

            return true;
        }
        public async Task<bool> DodajUplatuAsync(
            int id,
            decimal iznos,
            string korisnikId)
        {
            if (iznos <= 0)
            {
                throw new ArgumentException(
                    "Iznos uplate mora biti veći od nule.");
            }

            CiljStednje? postojeci =
                await GetByIdAsync(
                    id,
                    korisnikId);

            if (postojeci == null)
            {
                return false;
            }

            if (postojeci.Status == StatusCilja.Ostvaren)
            {
                throw new InvalidOperationException(
                    "Ovaj cilj je već ostvaren.");
            }

            if (DateTime.UtcNow.Date >
                postojeci.DatumZavrsetka.Date)
            {
                throw new InvalidOperationException(
                    "Rok za ovaj cilj je istekao.");
            }

            decimal preostaliIznos =
                postojeci.CiljaniIznos -
                postojeci.TrenutnoUstedjeno;

            if (iznos > preostaliIznos)
            {
                throw new ArgumentException(
                    $"Najveća moguća uplata je " +
                    $"{preostaliIznos:N2} KM.");
            }

           

            postojeci.TrenutnoUstedjeno += iznos;

            AzurirajStatus(postojeci);

            _repository.Update(postojeci);
            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> OduzmiUplatuAsync(
            int id,
            decimal iznos,
            string korisnikId)
        {
            if (iznos <= 0)
            {
                throw new ArgumentException(
                    "Iznos mora biti veći od nule.");
            }

            CiljStednje? postojeci =
                await GetByIdAsync(
                    id,
                    korisnikId);

            if (postojeci == null)
            {
                return false;
            }

            if (iznos > postojeci.TrenutnoUstedjeno)
            {
                throw new ArgumentException(
                    "Nije moguće oduzeti više od trenutno ušteđenog iznosa.");
            }

            postojeci.TrenutnoUstedjeno -= iznos;

            AzurirajStatus(postojeci);

            _repository.Update(postojeci);
            await _repository.SaveChangesAsync();

            return true;
        }

        public decimal IzracunajPostotak(
            CiljStednje ciljStednje)
        {
            if (ciljStednje.CiljaniIznos <= 0)
            {
                return 0;
            }

            decimal postotak =
                ciljStednje.TrenutnoUstedjeno /
                ciljStednje.CiljaniIznos *
                100;

            return Math.Min(
                Math.Round(postotak, 2),
                100);
        }

        private async Task<decimal> IzracunajRaspolozivaSredstvaAsync(
    string korisnikId)
        {
            IEnumerable<IzvorPrihoda> prihodi =
                await _izvorPrihodaService
                    .GetAllAsync(korisnikId);

            IEnumerable<Transakcija> transakcije =
                await _transakcijaRepository
                    .FindAsync(
                        t => t.KorisnikId == korisnikId);

            IEnumerable<CiljStednje> ciljevi =
                await GetAllAsync(korisnikId);

            decimal ukupniPrihodi =
                prihodi.Sum(
                    p => p.MjesecniIznos);

            decimal ukupniRashodi =
                transakcije
                    .Where(t =>
                        t.Vrsta == VrstaTransakcije.FiksniTrosak ||
                        t.Vrsta == VrstaTransakcije.DnevnaPotrosnja)
                    .Sum(t => t.Iznos);

            decimal ukupnoUstedjeno =
                ciljevi.Sum(
                    c => c.TrenutnoUstedjeno);

            decimal raspolozivo =
                ukupniPrihodi -
                ukupniRashodi -
                ukupnoUstedjeno;

            return Math.Max(0, raspolozivo);
        }
        private static void Validiraj(
            CiljStednje ciljStednje)
        {
            if (string.IsNullOrWhiteSpace(
                ciljStednje.Naziv))
            {
                throw new ArgumentException(
                    "Naziv cilja štednje je obavezan.");
            }

            if (ciljStednje.CiljaniIznos <= 0)
            {
                throw new ArgumentException(
                    "Ciljani iznos mora biti veći od nule.");
            }

            if (ciljStednje.DatumZavrsetka.Date <=
                DateTime.UtcNow.Date)
            {
                throw new ArgumentException(
                    "Rok cilja mora biti nakon današnjeg datuma.");
            }
        }

        private static void AzurirajStatus(
            CiljStednje ciljStednje)
        {
            if (ciljStednje.TrenutnoUstedjeno >=
                ciljStednje.CiljaniIznos)
            {
                ciljStednje.Status =
                    StatusCilja.Ostvaren;
            }
            else if (DateTime.UtcNow.Date >
                     ciljStednje.DatumZavrsetka.Date)
            {
                ciljStednje.Status =
                    StatusCilja.Neuspjesan;
            }
            else
            {
                ciljStednje.Status =
                    StatusCilja.UToku;
            }
        }
    }
}