namespace UstediPametno.ViewModels
{
    public class MjesecniPlanPrikazViewModel
    {
        public int Id { get; set; }

        public int Godina { get; set; }

        public int Mjesec { get; set; }

        public List<MjesecniPlanCiljPrikazViewModel> Ciljevi { get; set; }
            = new();
        public decimal UkupanPrihod { get; set; }
        public decimal PrenesenoIzPrethodnogMjeseca { get; set; }

        public decimal UkupniFiksniTroskovi { get; set; }

        public decimal PotrebnaStednja { get; set; }

        public decimal StvarnoPotroseno { get; set; }

        public decimal StvarnoUstedjeno { get; set; }

        public decimal RaspolozivoZaTrosenje { get; set; }

        public decimal DnevniBudzet { get; set; }

    }
}