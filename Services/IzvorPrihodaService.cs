using UstediPametno.Models;
using UstediPametno.Repositories;

namespace UstediPametno.Services
{
    public class IzvorPrihodaService : IIzvorPrihodaService
    {
        private readonly IGenericRepository<IzvorPrihoda> _repository;
       


        public IzvorPrihodaService(
            IGenericRepository<IzvorPrihoda> repository)
        {
            _repository = repository;
           
        }
        public async Task<IEnumerable<IzvorPrihoda>> GetAllAsync(
            string korisnikId)
        {
            return await _repository.FindAsync(
                prihod => prihod.KorisnikId == korisnikId);
        }

        public async Task<IzvorPrihoda?> GetByIdAsync(
            int id,
            string korisnikId)
        {
            var prihodi = await _repository.FindAsync(
                prihod => prihod.Id == id &&
                          prihod.KorisnikId == korisnikId);

            return prihodi.FirstOrDefault();
        }

        public async Task AddAsync(
            IzvorPrihoda izvorPrihoda,
            string korisnikId)
        {
            Validiraj(izvorPrihoda);

            izvorPrihoda.KorisnikId = korisnikId;
            izvorPrihoda.DatumKreiranja = DateTime.UtcNow;

            await _repository.AddAsync(izvorPrihoda);
            await _repository.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(
            IzvorPrihoda izvorPrihoda,
            string korisnikId)
        {
            Validiraj(izvorPrihoda);

            var postojeci = await GetByIdAsync(
                izvorPrihoda.Id,
                korisnikId);

            if (postojeci == null)
            {
                return false;
            }

            postojeci.Naziv = izvorPrihoda.Naziv.Trim();
            postojeci.MjesecniIznos = izvorPrihoda.MjesecniIznos;

            _repository.Update(postojeci);
            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(
            int id,
            string korisnikId)
        {
            var postojeci = await GetByIdAsync(id, korisnikId);

            if (postojeci == null)
            {
                return false;
            }

            _repository.Remove(postojeci);
            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<decimal> GetUkupanMjesecniPrihodAsync(
            string korisnikId)
        {
            var prihodi = await GetAllAsync(korisnikId);

            return prihodi.Sum(
                prihod => prihod.MjesecniIznos);
        }

        private static void Validiraj(
            IzvorPrihoda izvorPrihoda)
        {
            if (string.IsNullOrWhiteSpace(izvorPrihoda.Naziv))
            {
                throw new ArgumentException(
                    "Naziv izvora prihoda je obavezan.");
            }

            if (izvorPrihoda.MjesecniIznos <= 0)
            {
                throw new ArgumentException(
                    "Mjesečni iznos mora biti veći od nule.");
            }
        }
    }
}