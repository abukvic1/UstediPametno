using UstediPametno.Models;

namespace UstediPametno.ViewModels
{
    public class BedzPrikazViewModel
    {
        public int Id { get; set; }

        public string Naziv { get; set; } = string.Empty;
        public string Opis { get; set; } = string.Empty;
        public string Ikona { get; set; } = string.Empty;

        public VrstaBedza Vrsta { get; set; }

        public decimal Prag { get; set; }

        public bool Osvojen { get; set; }

        public DateTime? DatumOsvajanja { get; set; }

        public decimal TrenutnaVrijednost { get; set; }

        public decimal NapredakProcenat { get; set; }
    }
}