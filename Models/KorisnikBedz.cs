using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UstediPametno.Models
{
    public class KorisnikBedz
    {
        [Key]
        public int Id { get; set; }

        public string KorisnikId { get; set; } = string.Empty;

        public int BedzId { get; set; }

        public DateTime DatumOsvajanja { get; set; }

        [ForeignKey(nameof(KorisnikId))]
        public Korisnik Korisnik { get; set; } = null!;

        [ForeignKey(nameof(BedzId))]
        public Bedz Bedz { get; set; } = null!;
    }
}