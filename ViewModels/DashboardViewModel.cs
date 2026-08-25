using UstediPametno.Models;

namespace UstediPametno.ViewModels
{
    public class DashboardViewModel
    {
        public decimal TrenutnoStanje { get; set; }

        public decimal Raspolozivo { get; set; }

        public decimal PrimljenoOvajMjesec { get; set; }

        public decimal PotrosenoOvajMjesec { get; set; }

        public decimal UstedjenoOvajMjesec { get; set; }

        public decimal UkupnoUstedjeno { get; set; }

        public decimal DnevniBudzet { get; set; }

        public int Godina { get; set; }

        public int Mjesec { get; set; }

        public List<DashboardCiljViewModel> Ciljevi { get; set; }
            = new();
    }

    public class DashboardCiljViewModel
    {
        public int Id { get; set; }

        public string Naziv { get; set; } = string.Empty;

        public decimal CiljaniIznos { get; set; }

        public decimal TrenutnoUstedjeno { get; set; }

        public decimal Preostalo { get; set; }

        public decimal NapredakPostotak { get; set; }

        public DateTime DatumZavrsetka { get; set; }

        public StatusCilja Status { get; set; }
    }
}