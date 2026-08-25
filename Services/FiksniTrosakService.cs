using UstediPametno.Models;
using UstediPametno.Repositories;

namespace UstediPametno.Services
{
    public class FiksniTrosakService : IFiksniTrosakService
    {
        private readonly IGenericRepository<FiksniTrosak> _repository;

        public FiksniTrosakService(
            IGenericRepository<FiksniTrosak> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<FiksniTrosak>> GetAllAsync(
            string korisnikId)
        {
            return await _repository.FindAsync(
                trosak => trosak.KorisnikId == korisnikId);
        }

        public async Task<IEnumerable<FiksniTrosak>> GetAktivneAsync(
            string korisnikId)
        {
            return await _repository.FindAsync(
                trosak => trosak.KorisnikId == korisnikId &&
                          trosak.JeAktivan);
        }

        public async Task<FiksniTrosak?> GetByIdAsync(
            int id,
            string korisnikId)
        {
            IEnumerable<FiksniTrosak> troskovi =
                await _repository.FindAsync(
                    trosak => trosak.Id == id &&
                              trosak.KorisnikId == korisnikId);

            return troskovi.FirstOrDefault();
        }

        public async Task AddAsync(
            FiksniTrosak fiksniTrosak,
            string korisnikId)
        {
            Validiraj(fiksniTrosak);

            fiksniTrosak.Naziv = fiksniTrosak.Naziv.Trim();
            fiksniTrosak.KorisnikId = korisnikId;
            fiksniTrosak.DatumKreiranja = DateTime.UtcNow;
            fiksniTrosak.JeAktivan = true;

            await _repository.AddAsync(fiksniTrosak);
            await _repository.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(
            FiksniTrosak fiksniTrosak,
            string korisnikId)
        {
            Validiraj(fiksniTrosak);

            FiksniTrosak? postojeci =
                await GetByIdAsync(fiksniTrosak.Id, korisnikId);

            if (postojeci == null)
            {
                return false;
            }

            postojeci.Naziv = fiksniTrosak.Naziv.Trim();
            postojeci.Kategorija = fiksniTrosak.Kategorija;
            postojeci.MjesecniIznos = fiksniTrosak.MjesecniIznos;
            postojeci.JeAktivan = fiksniTrosak.JeAktivan;

            _repository.Update(postojeci);
            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(
            int id,
            string korisnikId)
        {
            FiksniTrosak? postojeci =
                await GetByIdAsync(id, korisnikId);

            if (postojeci == null)
            {
                return false;
            }

            _repository.Remove(postojeci);
            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<decimal> GetUkupanMjesecniTrosakAsync(
            string korisnikId)
        {
            IEnumerable<FiksniTrosak> aktivniTroskovi =
                await GetAktivneAsync(korisnikId);

            return aktivniTroskovi.Sum(
                trosak => trosak.MjesecniIznos);
        }

        private static void Validiraj(FiksniTrosak fiksniTrosak)
        {
            if (string.IsNullOrWhiteSpace(fiksniTrosak.Naziv))
            {
                throw new ArgumentException(
                    "Naziv fiksnog troška je obavezan.");
            }

            if (fiksniTrosak.MjesecniIznos <= 0)
            {
                throw new ArgumentException(
                    "Mjesečni iznos mora biti veći od nule.");
            }

            if (!Enum.IsDefined(
                typeof(KategorijaTroska),
                fiksniTrosak.Kategorija))
            {
                throw new ArgumentException(
                    "Odabrana kategorija troška nije ispravna.");
            }
        }
    }
}
