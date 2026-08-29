using UstediPametno.Models;
using UstediPametno.ViewModels;
namespace UstediPametno.Services
{
    public interface IBedzService
    {
        Task ProvjeriBedzeveAsync(string korisnikId);

        Task<IEnumerable<BedzPrikazViewModel>> GetBedzeveKorisnikaAsync(
    string korisnikId);
    }
}