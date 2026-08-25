using Microsoft.AspNetCore.Identity;

namespace UstediPametno.Models
{
    public class Korisnik : IdentityUser
    {
        public string ImePrezime { get; set; } = string.Empty;

        public DateTime DatumKreiranja { get; set; } = DateTime.UtcNow;
    }
}