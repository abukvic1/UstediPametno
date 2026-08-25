using System.ComponentModel.DataAnnotations;

namespace UstediPametno.ViewModels
{
    public class KreirajMjesecniPlanViewModel
    {
        [Range(
            2000,
            2100,
            ErrorMessage = "Godina nije ispravna.")]
        public int Godina { get; set; }

        [Range(
            1,
            12,
            ErrorMessage = "Mjesec mora biti između 1 i 12.")]
        public int Mjesec { get; set; }
    }
}