using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UstediPametno.Models;
using UstediPametno.Services;

namespace UstediPametno.Controllers
{
    [Authorize]
    public class IzvorPrihodaController : Controller
    {
        private readonly IIzvorPrihodaService _service;
        private readonly UserManager<Korisnik> _userManager;
        private readonly IMjesecniPlanService _planService;

        public IzvorPrihodaController(
            IIzvorPrihodaService prihodService,
            IMjesecniPlanService planService,
            UserManager<Korisnik> userManager)
        {
            _service = prihodService;
            _planService = planService;
            _userManager = userManager;
        }


        private async Task PonovoIzracunajPlanoveAsync(
            string korisnikId)
        {
            var planovi =
                await _planService.GetAllAsync(korisnikId);

            DateTime danas = DateTime.Now;

            foreach (var plan in planovi)
            {
                bool trenutniIliBuduciPlan =
                    plan.Godina > danas.Year ||
                    (plan.Godina == danas.Year &&
                     plan.Mjesec >= danas.Month);

                if (trenutniIliBuduciPlan)
                {
                    await _planService.PonovoIzracunajAsync(
                        plan.Id,
                        korisnikId);
                }
            }
        }


        public async Task<IActionResult> Index()
        {
            string? korisnikId =
                _userManager.GetUserId(User);

            if (korisnikId == null)
            {
                return Challenge();
            }

            var prihodi =
                await _service.GetAllAsync(korisnikId);

            return View(prihodi);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Naziv,MjesecniIznos")]
            IzvorPrihoda izvorPrihoda)
        {
            if (!ModelState.IsValid)
            {
                return View(izvorPrihoda);
            }

            string? korisnikId =
                _userManager.GetUserId(User);

            if (korisnikId == null)
            {
                return Challenge();
            }

            try
            {
                await _service.AddAsync(
                    izvorPrihoda,
                    korisnikId);

                await PonovoIzracunajPlanoveAsync(
                    korisnikId);

                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    exception.Message);

                return View(izvorPrihoda);
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

            IzvorPrihoda? izvorPrihoda =
                await _service.GetByIdAsync(
                    id,
                    korisnikId);

            if (izvorPrihoda == null)
            {
                return NotFound();
            }

            return View(izvorPrihoda);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Naziv,MjesecniIznos")]
            IzvorPrihoda izvorPrihoda)
        {
            if (id != izvorPrihoda.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(izvorPrihoda);
            }

            string? korisnikId =
                _userManager.GetUserId(User);

            if (korisnikId == null)
            {
                return Challenge();
            }

            try
            {
                bool uspjesno =
                    await _service.UpdateAsync(
                        izvorPrihoda,
                        korisnikId);

                if (!uspjesno)
                {
                    return NotFound();
                }

                await PonovoIzracunajPlanoveAsync(
                    korisnikId);

                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    exception.Message);

                return View(izvorPrihoda);
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

            IzvorPrihoda? izvorPrihoda =
                await _service.GetByIdAsync(
                    id,
                    korisnikId);

            if (izvorPrihoda == null)
            {
                return NotFound();
            }

            return View(izvorPrihoda);
        }


        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string? korisnikId =
                _userManager.GetUserId(User);

            if (korisnikId == null)
            {
                return Challenge();
            }

            bool uspjesno =
                await _service.DeleteAsync(
                    id,
                    korisnikId);

            if (!uspjesno)
            {
                return NotFound();
            }

            await PonovoIzracunajPlanoveAsync(
                korisnikId);

            return RedirectToAction(nameof(Index));
        }
    }
}