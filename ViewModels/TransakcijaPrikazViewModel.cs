using UstediPametno.Models;

namespace UstediPametno.ViewModels
{
    public class TransakcijaPrikazViewModel
    {
        public int Id { get; set; }

        public DateTime Datum { get; set; }

        public string Opis { get; set; } = string.Empty;

        public VrstaTransakcije Vrsta { get; set; }

        public decimal Iznos { get; set; }

        public string NazivPlana { get; set; } = "Nema plana";

        public string NazivCilja { get; set; } = "-";
    }
}