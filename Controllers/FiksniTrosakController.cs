using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UstediPametno.Models;
using UstediPametno.Services;

namespace UstediPametno.Controllers
{
    [Authorize]
    public class FiksniTrosakController : Controller
    {
        private readonly IFiksniTrosakService _service;
        private readonly UserManager<Korisnik> _userManager;

        public FiksniTrosakController(
            IFiksniTrosakService service,
            UserManager<Korisnik> userManager)
        {
            _service = service;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            string? korisnikId = _userManager.GetUserId(User);

            if (korisnikId == null)
            {
                return Challenge();
            }

            IEnumerable<FiksniTrosak> troskovi =
                await _service.GetAllAsync(korisnikId);

            return View(troskovi);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Naziv,Kategorija,MjesecniIznos")]
            FiksniTrosak fiksniTrosak)
        {
            if (!ModelState.IsValid)
            {
                return View(fiksniTrosak);
            }

            string? korisnikId = _userManager.GetUserId(User);

            if (korisnikId == null)
            {
                return Challenge();
            }

            try
            {
                await _service.AddAsync(
                    fiksniTrosak,
                    korisnikId);

                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    exception.Message);

                return View(fiksniTrosak);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            string? korisnikId = _userManager.GetUserId(User);

            if (korisnikId == null)
            {
                return Challenge();
            }

            FiksniTrosak? fiksniTrosak =
                await _service.GetByIdAsync(id, korisnikId);

            if (fiksniTrosak == null)
            {
                return NotFound();
            }

            return View(fiksniTrosak);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
    int id,
    [Bind("Id,Naziv,Kategorija,MjesecniIznos,JeAktivan")]
    FiksniTrosak fiksniTrosak)
        {
            if (id != fiksniTrosak.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(fiksniTrosak);
            }

            string? korisnikId = _userManager.GetUserId(User);

            if (korisnikId == null)
            {
                return Challenge();
            }

            try
            {
                bool uspjesno =
                    await _service.UpdateAsync(
                        fiksniTrosak,
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

                return View(fiksniTrosak);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            string? korisnikId = _userManager.GetUserId(User);

            if (korisnikId == null)
            {
                return Challenge();
            }

            FiksniTrosak? fiksniTrosak =
                await _service.GetByIdAsync(id, korisnikId);

            if (fiksniTrosak == null)
            {
                return NotFound();
            }

            return View(fiksniTrosak);
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
                await _service.DeleteAsync(id, korisnikId);

            if (!uspjesno)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}