using System.ComponentModel.DataAnnotations;

namespace UstediPametno.Models
{
    public class Bedz
    {
        [Key]
        public int Id { get; set; }

        public string Naziv { get; set; } = string.Empty;

        public string Opis { get; set; } = string.Empty;

        public VrstaBedza Vrsta { get; set; }

        public decimal Prag { get; set; }

        public string Ikona { get; set; } = string.Empty;
    }
}