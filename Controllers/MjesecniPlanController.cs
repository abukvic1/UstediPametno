using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using UstediPametno.Models;
using UstediPametno.Services;
using UstediPametno.ViewModels;

namespace UstediPametno.Controllers
{
    [Authorize]
    public class MjesecniPlanController : Controller
    {
        private readonly IMjesecniPlanService _planService;
        private readonly ICiljStednjeService _ciljService;
        private readonly UserManager<Korisnik> _userManager;
        private readonly IMjesecniPlanCiljService
    _planCiljService;
        private readonly ITransakcijaService _transakcijaService;
        public MjesecniPlanController(
     IMjesecniPlanService planService,
     IMjesecniPlanCiljService planCiljService,
     ICiljStednjeService ciljService,
     ITransakcijaService transakcijaService,
     UserManager<Korisnik> userManager)
        {
            _planService = planService;
            _planCiljService = planCiljService;
            _ciljService = ciljService;
            _transakcijaService = transakcijaService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            string? korisnikId =
                _userManager.GetUserId(User);

            if (korisnikId == null)
            {
                return Challenge();
            }

            IEnumerable<MjesecniPlan> planovi =
                await _planService.GetAllAsync(korisnikId);

            foreach (MjesecniPlan plan in planovi)
            {
                await _planService.PonovoIzracunajAsync(
                    plan.Id,
                    korisnikId);
            }

            planovi =
                await _planService.GetAllAsync(korisnikId);

            IEnumerable<CiljStednje> sviCiljevi =
                await _ciljService.GetAllAsync(korisnikId);

            List<MjesecniPlanPrikazViewModel> model = new();

            foreach (MjesecniPlan plan in planovi)
            {
                IEnumerable<Transakcija> transakcijePlana =
                    await _transakcijaService.GetByPlanAsync(
                        plan.Id,
                        korisnikId);

                IEnumerable<MjesecniPlanCilj> veze =
                    await _planCiljService.GetByPlanAsync(
                        plan.Id);

                List<MjesecniPlanCiljPrikazViewModel> ciljeviPlana =
                    veze
                        .Select(veza =>
                        {
                            CiljStednje? cilj =
                                sviCiljevi.FirstOrDefault(
                                    c => c.Id == veza.CiljStednjeId);

                            if (cilj == null)
                            {
                                return null;
                            }

                            decimal uplaceno =
                                transakcijePlana
                                    .Where(t =>
                                        t.Vrsta ==
                                            VrstaTransakcije.UplataStednje &&
                                        t.CiljStednjeId == cilj.Id)
                                    .Sum(t => t.Iznos);

                            decimal preostalo =
                                Math.Max(
                                    0,
                                    veza.PlaniraniIznos -
                                    uplaceno);

                            decimal napredak =
                                veza.PlaniraniIznos > 0
                                    ? Math.Min(
                                        100,
                                        uplaceno /
                                        veza.PlaniraniIznos *
                                        100)
                                    : 0;

                            return new MjesecniPlanCiljPrikazViewModel
                            {
                                Naziv = cilj.Naziv,

                                PlaniraniIznos =
                                    veza.PlaniraniIznos,

                                Uplaceno =
                                    uplaceno,

                                Preostalo =
                                    preostalo,

                                NapredakPostotak =
                                    Math.Round(
                                        napredak,
                                        2)
                            };
                        })
                        .Where(cilj => cilj != null)
                        .Select(cilj => cilj!)
                        .ToList();

                MjesecniPlanPrikazViewModel prikaz =
                    new MjesecniPlanPrikazViewModel
                    {
                        Id = plan.Id,

                        Godina = plan.Godina,

                        Mjesec = plan.Mjesec,

                        Ciljevi = ciljeviPlana,

                        UkupanPrihod =
                            plan.UkupanPrihod,

                        PrenesenoIzPrethodnogMjeseca =
                            plan.PrenesenoIzPrethodnogMjeseca,

                        UkupniFiksniTroskovi =
                            plan.UkupniFiksniTroskovi,

                        PotrebnaStednja =
                            plan.PotrebnaStednja,

                        StvarnoPotroseno =
                            plan.StvarnoPotroseno,

                        StvarnoUstedjeno =
                            plan.StvarnoUstedjeno,

                        RaspolozivoZaTrosenje =
                            plan.RaspolozivoZaTrosenje,

                        DnevniBudzet =
                            plan.DnevniBudzet
                    };

                model.Add(prikaz);
            }

            return View(model);
        }
        [HttpGet]
        public IActionResult Create()
        {
            DateTime danas = DateTime.Today;

            KreirajMjesecniPlanViewModel model =
                new KreirajMjesecniPlanViewModel
                {
                    Godina = danas.Year,
                    Mjesec = danas.Month
                };

            return View(model);
        }



       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
      [Bind("Godina,Mjesec")]
    KreirajMjesecniPlanViewModel model)
        {
            string? korisnikId =
                _userManager.GetUserId(User);

            if (korisnikId == null)
            {
                return Challenge();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                MjesecniPlan plan =
                    await _planService.KreirajAsync(
                        model.Godina,
                        model.Mjesec,
                        korisnikId);

                await _planCiljService.DodajAktivneCiljeveZaPlanAsync(
                    plan.Id,
                    model.Godina,
                    model.Mjesec,
                    korisnikId);

                await _planService.PonovoIzracunajAsync(
                    plan.Id,
                    korisnikId);

                return RedirectToAction(
                    nameof(Index));
            }
            catch (ArgumentException exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    exception.Message);

                return View(model);
            }
            catch (InvalidOperationException exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    exception.Message);

                return View(model);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PonovoIzracunaj(int id)
        {
            string? korisnikId = _userManager.GetUserId(User);

            if (korisnikId == null)
            {
                return Challenge();
            }

            bool uspjesno =
                await _planService.PonovoIzracunajAsync(
                    id,
                    korisnikId);

            if (!uspjesno)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            string? korisnikId = _userManager.GetUserId(User);

            if (korisnikId == null)
            {
                return Challenge();
            }

            MjesecniPlan? plan =
                await _planService.GetByIdAsync(id, korisnikId);

            if (plan == null)
            {
                return NotFound();
            }

            return View(plan);
        }
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string? korisnikId = _userManager.GetUserId(User);

            if (korisnikId == null)
            {
                return Challenge();
            }

            bool uspjesno =
                await _planService.DeleteAsync(id, korisnikId);

            if (!uspjesno)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}