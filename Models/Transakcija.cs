using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
namespace UstediPametno.Models
{
    public class Transakcija
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Korisnik")]
        public string KorisnikId { get; set; } = string.Empty;
        [ValidateNever]
        public Korisnik Korisnik { get; set; } = null!;

        [ForeignKey("MjesecniPlan")]
        public int? MjesecniPlanId { get; set; }
        public MjesecniPlan? MjesecniPlan { get; set; }

        [ForeignKey("CiljStednje")]
        public int? CiljStednjeId { get; set; }
        public CiljStednje? CiljStednje { get; set; }

        public VrstaTransakcije Vrsta { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Iznos { get; set; }
        public string Opis { get; set; } = string.Empty;
        public DateTime Datum { get; set; }

        public Transakcija() { }
    }
}
