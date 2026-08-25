using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using UstediPametno.Models;

namespace UstediPametno.ViewModels
{
    public class TransakcijaFormaViewModel
    {
        public int Id { get; set; }

        public int? MjesecniPlanId { get; set; }

        public int? CiljStednjeId { get; set; }

        public VrstaTransakcije Vrsta { get; set; }

        [Range(
            typeof(decimal),
            "0.01",
            "9999999999999999",
            ErrorMessage = "Iznos mora biti veći od nule.")]
        public decimal Iznos { get; set; }

        [Required(ErrorMessage = "Opis je obavezan.")]
        [StringLength(
            200,
            ErrorMessage = "Opis može imati najviše 200 znakova.")]
        public string Opis { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime Datum { get; set; } = DateTime.Today;

        public List<SelectListItem> Planovi { get; set; } =
            new List<SelectListItem>();

        public List<SelectListItem> Ciljevi { get; set; } =
            new List<SelectListItem>();
    }
}