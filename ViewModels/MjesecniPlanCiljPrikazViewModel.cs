namespace UstediPametno.ViewModels
{
    public class MjesecniPlanCiljPrikazViewModel
    {
        public string Naziv { get; set; } = string.Empty;

        public decimal PlaniraniIznos { get; set; }

        public decimal Uplaceno { get; set; }

        public decimal Preostalo { get; set; }

        public decimal NapredakPostotak { get; set; }
    }
}