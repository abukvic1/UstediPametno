using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UstediPametno.Models;
using UstediPametno.Services;
using UstediPametno.ViewModels;

namespace UstediPametno.Controllers
{
    [Authorize]
    public class TransakcijaController : Controller
    {
        private readonly ITransakcijaService _transakcijaService;
        private readonly ICiljStednjeService _ciljService;
        private readonly UserManager<Korisnik> _userManager;
        private readonly IMjesecniPlanService _planService;
        public TransakcijaController(
    ITransakcijaService transakcijaService,
    ICiljStednjeService ciljService,
    IMjesecniPlanService planService,
    UserManager<Korisnik> userManager)
        {
            _transakcijaService = transakcijaService;
            _ciljService = ciljService;
            _planService = planService;
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

            IEnumerable<Transakcija> transakcije =
                await _transakcijaService.GetAllAsync(
                    korisnikId);

            IEnumerable<MjesecniPlan> planovi =
                await _planService.GetAllAsync(
                    korisnikId);

            IEnumerable<CiljStednje> ciljevi =
                await _ciljService.GetAllAsync(
                    korisnikId);

            List<TransakcijaPrikazViewModel> model =
                transakcije.Select(transakcija =>
                {
                    MjesecniPlan? plan = null;

                    if (transakcija.MjesecniPlanId.HasValue)
                    {
                        plan = planovi.FirstOrDefault(
                            p => p.Id ==
                                 transakcija.MjesecniPlanId.Value);
                    }

                    // Ako stara transakcija još nema PlanId,
                    // pronađi plan prema datumu.
                    if (plan == null)
                    {
                        plan = planovi.FirstOrDefault(
                            p =>
                                p.Godina == transakcija.Datum.Year &&
                                p.Mjesec == transakcija.Datum.Month);
                    }

                    CiljStednje? cilj = null;

                    if (transakcija.CiljStednjeId.HasValue)
                    {
                        cilj = ciljevi.FirstOrDefault(
                            c => c.Id ==
                                 transakcija.CiljStednjeId.Value);
                    }

                    string nazivPlana = "Nema plana";

                    if (plan != null)
                    {
                        string mjesec =
                            new DateTime(
                                plan.Godina,
                                plan.Mjesec,
                                1)
                            .ToString(
                                "MMMM",
                                new System.Globalization.CultureInfo("bs-BA"));

                        nazivPlana =
                            $"{mjesec} {plan.Godina}.";
                    }

                    return new TransakcijaPrikazViewModel
                    {
                        Id = transakcija.Id,
                        Datum = transakcija.Datum,
                        Opis = transakcija.Opis,
                        Vrsta = transakcija.Vrsta,
                        Iznos = transakcija.Iznos,

                        NazivPlana = nazivPlana,

                        NazivCilja =
                            cilj?.Naziv ?? "-"
                    };
                })
                .ToList();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            string? korisnikId =
                _userManager.GetUserId(User);

            if (korisnikId == null)
            {
                return Challenge();
            }

            TransakcijaFormaViewModel model =
                new TransakcijaFormaViewModel
                {
                    Datum = DateTime.Today
                };

            await PopuniCiljeveAsync(
                model,
                korisnikId);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("CiljStednjeId,Vrsta,Iznos,Opis,Datum")]
            TransakcijaFormaViewModel model)
        {
            string? korisnikId =
                _userManager.GetUserId(User);

            if (korisnikId == null)
            {
                return Challenge();
            }

            await PopuniCiljeveAsync(
                model,
                korisnikId);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Transakcija transakcija =
                new Transakcija
                {
                    CiljStednjeId =
                        model.CiljStednjeId,

                    Vrsta =
                        model.Vrsta,

                    Iznos =
                        model.Iznos,

                    Opis =
                        model.Opis,

                    Datum =
                        model.Datum
                };

            try
            {
                await _transakcijaService.AddAsync(
                    transakcija,
                    korisnikId);

                return RedirectToAction(nameof(Index));
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

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            string? korisnikId =
                _userManager.GetUserId(User);

            if (korisnikId == null)
            {
                return Challenge();
            }

            Transakcija? transakcija =
                await _transakcijaService.GetByIdAsync(
                    id,
                    korisnikId);

            if (transakcija == null)
            {
                return NotFound();
            }

            TransakcijaFormaViewModel model =
                new TransakcijaFormaViewModel
                {
                    Id =
                        transakcija.Id,

                    CiljStednjeId =
                        transakcija.CiljStednjeId,

                    Vrsta =
                        transakcija.Vrsta,

                    Iznos =
                        transakcija.Iznos,

                    Opis =
                        transakcija.Opis,

                    Datum =
                        transakcija.Datum
                };

            await PopuniCiljeveAsync(
                model,
                korisnikId);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind(
                "Id,CiljStednjeId,Vrsta," +
                "Iznos,Opis,Datum")]
            TransakcijaFormaViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            string? korisnikId =
                _userManager.GetUserId(User);

            if (korisnikId == null)
            {
                return Challenge();
            }

            await PopuniCiljeveAsync(
                model,
                korisnikId);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Transakcija transakcija =
                new Transakcija
                {
                    Id =
                        model.Id,

                    CiljStednjeId =
                        model.CiljStednjeId,

                    Vrsta =
                        model.Vrsta,

                    Iznos =
                        model.Iznos,

                    Opis =
                        model.Opis,

                    Datum =
                        model.Datum
                };

            try
            {
                bool uspjesno =
                    await _transakcijaService.UpdateAsync(
                        transakcija,
                        korisnikId);

                if (!uspjesno)
                {
                    return NotFound();
                }

                return RedirectToAction(nameof(Index));
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

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            string? korisnikId =
                _userManager.GetUserId(User);

            if (korisnikId == null)
            {
                return Challenge();
            }

            Transakcija? transakcija =
                await _transakcijaService.GetByIdAsync(
                    id,
                    korisnikId);

            if (transakcija == null)
            {
                return NotFound();
            }

            return View(transakcija);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(
            int id)
        {
            string? korisnikId =
                _userManager.GetUserId(User);

            if (korisnikId == null)
            {
                return Challenge();
            }

            try
            {
                bool uspjesno =
                    await _transakcijaService.DeleteAsync(
                        id,
                        korisnikId);

                if (!uspjesno)
                {
                    return NotFound();
                }

                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException exception)
            {
                TempData["Greska"] =
                    exception.Message;

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException exception)
            {
                TempData["Greska"] =
                    exception.Message;

                return RedirectToAction(nameof(Index));
            }
        }

        private async Task PopuniCiljeveAsync(
            TransakcijaFormaViewModel model,
            string korisnikId)
        {
            IEnumerable<CiljStednje> sviCiljevi =
                await _ciljService.GetAllAsync(
                    korisnikId);

            IEnumerable<CiljStednje> dostupniCiljevi =
                sviCiljevi.Where(cilj =>
                    cilj.Status == StatusCilja.UToku ||
                    cilj.Status == StatusCilja.Prilagodjen ||
                    cilj.Id == model.CiljStednjeId);

            model.Ciljevi = dostupniCiljevi
                .Select(cilj => new SelectListItem
                {
                    Value =
                        cilj.Id.ToString(),

                    Text =
                        $"{cilj.Naziv} " +
                        $"({cilj.TrenutnoUstedjeno:N2} / " +
                        $"{cilj.CiljaniIznos:N2} KM)"
                })
                .ToList();
        }
    }
}