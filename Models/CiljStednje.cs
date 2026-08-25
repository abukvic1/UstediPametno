using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UstediPametno.Models
{
    public class CiljStednje
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Korisnik")]
        public string KorisnikId { get; set; } = string.Empty;
        [ValidateNever]
        public Korisnik Korisnik { get; set; } = null!;

        public string Naziv { get; set; } = string.Empty;
        [Column(TypeName = "decimal(18,2)")]
        public decimal CiljaniIznos { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TrenutnoUstedjeno { get; set; }
        public DateTime DatumPocetka { get; set; }
        public DateTime DatumZavrsetka { get; set; }
        public StatusCilja Status { get; set; }

        public CiljStednje() { }
    }
}
