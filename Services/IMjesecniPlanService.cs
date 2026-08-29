using UstediPametno.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace UstediPametno.Services
{
    public interface IMjesecniPlanService
    {
        Task<IEnumerable<MjesecniPlan>> GetAllAsync(
            string korisnikId);

        Task<MjesecniPlan?> GetByIdAsync(
            int id,
            string korisnikId);

        Task<MjesecniPlan?> GetByPeriodAsync(
            int godina,
            int mjesec,
            string korisnikId);
        Task PropagirajStanjeOdMjesecaAsync(
    int godina,
    int mjesec,
    string korisnikId);
        Task<MjesecniPlan> KreirajAsync(
     int godina,
     int mjesec,
     string korisnikId);
        Task<bool> AzurirajPlaniranuStednjuAsync(
    int id,
    decimal iznos,
    string korisnikId);
        Task<bool> PonovoIzracunajAsync(
            int id,
            string korisnikId);

        Task<bool> AzurirajStvarneIznoseAsync(
            int id,
            decimal stvarnoPotroseno,
            decimal stvarnoUstedjeno,
            string korisnikId);

        Task<bool> DeleteAsync(
            int id,
            string korisnikId);
    }
}