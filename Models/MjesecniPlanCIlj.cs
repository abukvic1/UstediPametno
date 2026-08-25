using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UstediPametno.Models
{
    public class MjesecniPlanCilj
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("MjesecniPlan")]
        public int MjesecniPlanId { get; set; }

        public MjesecniPlan MjesecniPlan { get; set; } = null!;

        [ForeignKey("CiljStednje")]
        public int CiljStednjeId { get; set; }

        public CiljStednje CiljStednje { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal PlaniraniIznos { get; set; }
    }
}