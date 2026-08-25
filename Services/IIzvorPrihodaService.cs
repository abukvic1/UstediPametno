using UstediPametno.Models;
namespace UstediPametno.Services
{
    public interface IIzvorPrihodaService
    {
        Task<IEnumerable<IzvorPrihoda>> GetAllAsync(string korisnikId);

        Task<IzvorPrihoda?> GetByIdAsync(
            int id,
            string korisnikId);

        Task AddAsync(
            IzvorPrihoda izvorPrihoda,
            string korisnikId);

        Task<bool> UpdateAsync(
            IzvorPrihoda izvorPrihoda,
            string korisnikId);

        Task<bool> DeleteAsync(
            int id,
            string korisnikId);

        Task<decimal> GetUkupanMjesecniPrihodAsync(
            string korisnikId);
    }
}
