namespace UstediPametno.ViewModels
{
    public class TransakcijeIndexViewModel
    {
        public IEnumerable<TransakcijaPrikazViewModel> Transakcije { get; set; }
            = new List<TransakcijaPrikazViewModel>();

        public decimal UkupniPrihodi { get; set; }

        public decimal UkupniRashodi { get; set; }

        public decimal UkupnoUstedjeno { get; set; }

        public decimal Raspolozivo { get; set; }
    }
}