using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
namespace UstediPametno.Models
{
    public class IzvorPrihoda
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Korisnik")]
        public string KorisnikId { get; set; } = string.Empty;
        [ValidateNever]
        public Korisnik Korisnik { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal MjesecniIznos { get; set; }
        public string Naziv { get; set; } = string.Empty;

     public DateTime DatumKreiranja { get; set; }

        public IzvorPrihoda() { }
    }
}
