using UstediPametno.Models;

namespace UstediPametno.Services
{
    public interface ITransakcijaService
    {
        Task<IEnumerable<Transakcija>> GetAllAsync(
            string korisnikId);

        Task<IEnumerable<Transakcija>> GetByPlanAsync(
            int mjesecniPlanId,
            string korisnikId);

        Task<Transakcija?> GetByIdAsync(
            int id,
            string korisnikId);

        Task AddAsync(
            Transakcija transakcija,
            string korisnikId);

        Task<bool> UpdateAsync(
            Transakcija transakcija,
            string korisnikId);

        Task<bool> DeleteAsync(
            int id,
            string korisnikId);

        Task<decimal> GetUkupnoPotrosenoZaPlanAsync(
            int mjesecniPlanId,
            string korisnikId);

        Task<decimal> GetUkupnoUstedjenoZaPlanAsync(
            int mjesecniPlanId,
            string korisnikId);
        Task<decimal> GetUkupneRashodeAsync(
    string korisnikId);
        Task<decimal> GetUkupnoUstedjenoAsync(
    string korisnikId);
        Task<decimal> GetUkupanPrihodAsync(
    string korisnikId);

    }
}