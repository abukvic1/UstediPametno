using System.ComponentModel.DataAnnotations;

namespace UstediPametno.ViewModels
{
    public class UplataNaCiljViewModel
    {
        public int Id { get; set; }

        public string Naziv { get; set; } = string.Empty;

        public decimal CiljaniIznos { get; set; }

        public decimal TrenutnoUstedjeno { get; set; }
        public decimal PreostaliIznos =>
    CiljaniIznos - TrenutnoUstedjeno;
        [Range(
            typeof(decimal),
            "0.01",
            "9999999999999999",
            ErrorMessage = "Iznos uplate mora biti veći od nule.")]
        public decimal Iznos { get; set; }
    }
}