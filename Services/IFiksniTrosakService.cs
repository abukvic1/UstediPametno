using UstediPametno.Models;

namespace UstediPametno.Services
{
    public interface IFiksniTrosakService
    {
        Task<IEnumerable<FiksniTrosak>> GetAllAsync(
            string korisnikId);

        Task<IEnumerable<FiksniTrosak>> GetAktivneAsync(
            string korisnikId);

        Task<FiksniTrosak?> GetByIdAsync(
            int id,
            string korisnikId);

        Task AddAsync(
            FiksniTrosak fiksniTrosak,
            string korisnikId);

        Task<bool> UpdateAsync(
            FiksniTrosak fiksniTrosak,
            string korisnikId);

        Task<bool> DeleteAsync(
            int id,
            string korisnikId);

        Task<decimal> GetUkupanMjesecniTrosakAsync(
            string korisnikId);
    }
}