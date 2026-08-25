using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UstediPametno.Models
{
    public class MjesecniPlan
    {
        [Key]
        public int Id { get; set; }
        public ICollection<MjesecniPlanCilj> Ciljevi { get; set; }
    = new List<MjesecniPlanCilj>();

        [ForeignKey("Korisnik")]
        public string KorisnikId { get; set; } = string.Empty;
        [ValidateNever]
        public Korisnik Korisnik { get; set; } = null!;

        [ForeignKey("CiljStednje")]
        public int? CiljStednjeId { get; set; }

        [ValidateNever]
        public CiljStednje? CiljStednje { get; set; }

        public int Godina { get; set; }
        public int Mjesec { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UkupanPrihod { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrenesenoIzPrethodnogMjeseca { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UkupniFiksniTroskovi { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PotrebnaStednja { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal RaspolozivoZaTrosenje { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DnevniBudzet { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal StvarnoPotroseno { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal StvarnoUstedjeno { get; set; }
        

        public MjesecniPlan() { }
    }
}
