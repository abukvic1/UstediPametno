using UstediPametno.Models;

namespace UstediPametno.Services
{
    public interface IMjesecniPlanCiljService
    {
        Task DodajCiljeveAsync(
            int mjesecniPlanId,
            IEnumerable<int> ciljIds,
            int godina,
            int mjesec,
            string korisnikId);


        Task<IEnumerable<MjesecniPlanCilj>> GetByPlanAsync(
            int mjesecniPlanId);

        Task<decimal> GetUkupnoPlaniranoAsync(
            int mjesecniPlanId);
        Task DodajAktivneCiljeveZaPlanAsync(
    int mjesecniPlanId,
    int godina,
    int mjesec,
    string korisnikId);
        Task SinhronizujCiljSaPlanovimaAsync(
    int ciljId,
    string korisnikId);
        Task ObrisiVezuAsync(
    int mjesecniPlanId,
    int ciljId);
    }

}