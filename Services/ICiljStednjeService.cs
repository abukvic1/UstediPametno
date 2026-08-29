using UstediPametno.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UstediPametno.Services
{
    public interface ICiljStednjeService
    {
        Task<IEnumerable<CiljStednje>> GetAllAsync(
            string korisnikId);

        Task<IEnumerable<CiljStednje>> GetAktivneAsync(
            string korisnikId);

        Task<CiljStednje?> GetByIdAsync(
            int id,
            string korisnikId);

        Task AddAsync(
            CiljStednje ciljStednje,
            string korisnikId);

        Task<bool> UpdateAsync(
            CiljStednje ciljStednje,
            string korisnikId);

        Task<bool> DeleteAsync(
            int id,
            string korisnikId);

        Task<bool> DodajUplatuAsync(
            int id,
            decimal iznos,
            string korisnikId);

        decimal IzracunajPostotak(
            CiljStednje ciljStednje);

        Task<bool> OduzmiUplatuAsync(
        int id,
        decimal iznos,
        string korisnikId);
    }
}