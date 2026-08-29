using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UstediPametno.Models;
using Microsoft.AspNetCore.Identity;

using UstediPametno.Services;
using UstediPametno.ViewModels;

namespace UstediPametno.Controllers
{
    public class HomeController : Controller

    {
        private readonly UserManager<Korisnik> _userManager;
        private readonly ITransakcijaService _transakcijaService;
        private readonly ICiljStednjeService _ciljService;
        private readonly IMjesecniPlanService _planService;
    
        public HomeController(
     UserManager<Korisnik> userManager,
     ITransakcijaService transakcijaService,
     ICiljStednjeService ciljService,
     IMjesecniPlanService planService)
        {
            _userManager = userManager;
            _transakcijaService = transakcijaService;
            _ciljService = ciljService;
            _planService = planService;
          
        }
        public async Task<IActionResult> Index()
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return View();
            }

            string? korisnikId =
                _userManager.GetUserId(User);

            if (korisnikId == null)
            {
                return Challenge();
            }
            
            DateTime danas =
                DateTime.Today;

            // ===========================
            // PRONAĐI TRENUTNI PLAN
            // ===========================

            MjesecniPlan? trenutniPlan =
                await _planService.GetByPeriodAsync(
                    danas.Year,
                    danas.Month,
                    korisnikId);

            // Ako plan postoji, prvo ga ponovo izračunaj
            // da uzme najnovije transakcije.
            if (trenutniPlan != null)
            {
                await _planService.PonovoIzracunajAsync(
                    trenutniPlan.Id,
                    korisnikId);

                // Ponovo učitaj plan iz baze sa novim vrijednostima
                trenutniPlan =
                    await _planService.GetByPeriodAsync(
                        danas.Year,
                        danas.Month,
                        korisnikId);
            }

            // ===========================
            // TRANSAKCIJE
            // ===========================

            IEnumerable<Transakcija> sveTransakcije =
                await _transakcijaService.GetAllAsync(
                    korisnikId);

            List<Transakcija> transakcijeOvogMjeseca =
                sveTransakcije
                    .Where(t =>
                        t.Datum.Year == danas.Year &&
                        t.Datum.Month == danas.Month)
                    .ToList();

            decimal ukupniPrihodi =
                sveTransakcije
                    .Where(t =>
                        t.Vrsta == VrstaTransakcije.Prihod)
                    .Sum(t => t.Iznos);

            decimal ukupniRashodi =
                sveTransakcije
                    .Where(t =>
                        t.Vrsta == VrstaTransakcije.FiksniTrosak ||
                        t.Vrsta == VrstaTransakcije.DnevnaPotrosnja)
                    .Sum(t => t.Iznos);

            decimal primljenoOvajMjesec =
                transakcijeOvogMjeseca
                    .Where(t =>
                        t.Vrsta == VrstaTransakcije.Prihod)
                    .Sum(t => t.Iznos);

            decimal potrosenoOvajMjesec =
                transakcijeOvogMjeseca
                    .Where(t =>
                        t.Vrsta == VrstaTransakcije.FiksniTrosak ||
                        t.Vrsta == VrstaTransakcije.DnevnaPotrosnja)
                    .Sum(t => t.Iznos);

            decimal ustedjenoOvajMjesec =
                transakcijeOvogMjeseca
                    .Where(t =>
                        t.Vrsta == VrstaTransakcije.UplataStednje)
                    .Sum(t => t.Iznos);

            // ===========================
            // CILJEVI
            // ===========================

            IEnumerable<CiljStednje> sviCiljevi =
                await _ciljService.GetAllAsync(
                    korisnikId);

            decimal ukupnoUstedjeno =
                sviCiljevi.Sum(
                    cilj => cilj.TrenutnoUstedjeno);

            decimal trenutnoStanje =
                ukupniPrihodi -
                ukupniRashodi -
                ukupnoUstedjeno;

            // ===========================
            // RASPOLOŽIVO
            // ===========================

            decimal raspolozivo;

            if (trenutniPlan != null)
            {
                raspolozivo =
                    trenutniPlan.RaspolozivoZaTrosenje;
            }
            else
            {
                raspolozivo =
                    Math.Max(
                        0,
                        trenutnoStanje);
            }

            // ===========================
            // DASHBOARD CILJEVI
            // ===========================

            List<DashboardCiljViewModel> ciljevi =
                sviCiljevi
                    .Select(cilj =>
                    {
                        decimal preostalo =
                            Math.Max(
                                0,
                                cilj.CiljaniIznos -
                                cilj.TrenutnoUstedjeno);

                        decimal napredak =
                            cilj.CiljaniIznos > 0
                                ? Math.Min(
                                    100,
                                    cilj.TrenutnoUstedjeno /
                                    cilj.CiljaniIznos *
                                    100)
                                : 0;

                        return new DashboardCiljViewModel
                        {
                            Id = cilj.Id,

                            Naziv = cilj.Naziv,

                            CiljaniIznos =
                                cilj.CiljaniIznos,

                            TrenutnoUstedjeno =
                                cilj.TrenutnoUstedjeno,

                            Preostalo =
                                preostalo,

                            NapredakPostotak =
                                Math.Round(
                                    napredak,
                                    2),

                            DatumZavrsetka =
                                cilj.DatumZavrsetka,

                            Status =
                                cilj.Status
                        };
                    })
                    .OrderByDescending(
                        cilj => cilj.NapredakPostotak)
                    .ToList();

            // ===========================
            // VIEW MODEL
            // ===========================

            DashboardViewModel model =
                new DashboardViewModel
                {
                    TrenutnoStanje =
                        Math.Round(
                            trenutnoStanje,
                            2),

                    Raspolozivo =
                        Math.Round(
                            raspolozivo,
                            2),

                    PrimljenoOvajMjesec =
                        Math.Round(
                            primljenoOvajMjesec,
                            2),

                    PotrosenoOvajMjesec =
                        Math.Round(
                            potrosenoOvajMjesec,
                            2),

                    UstedjenoOvajMjesec =
                        Math.Round(
                            ustedjenoOvajMjesec,
                            2),

                    UkupnoUstedjeno =
                        Math.Round(
                            ukupnoUstedjeno,
                            2),

                    DnevniBudzet =
                        trenutniPlan?.DnevniBudzet ?? 0,

                    Godina =
                        danas.Year,

                    Mjesec =
                        danas.Month,

                    Ciljevi =
                        ciljevi
                };

            return View(model);
        }
       
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
