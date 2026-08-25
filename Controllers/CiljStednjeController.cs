using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UstediPametno.Models;
using UstediPametno.Services;
using UstediPametno.ViewModels;
namespace UstediPametno.Controllers
{
    [Authorize]
    public class CiljStednjeController : Controller
    {
        private readonly ICiljStednjeService _service;
        private readonly UserManager<Korisnik> _userManager;

        public CiljStednjeController(
            ICiljStednjeService service,
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

            IEnumerable<CiljStednje> ciljevi =
                await _service.GetAllAsync(korisnikId);

            List<CiljStednjePrikazViewModel> prikaz =
                ciljevi.Select(cilj =>
                    new CiljStednjePrikazViewModel
                    {
                        Cilj = cilj,
                        Postotak = _service.IzracunajPostotak(cilj)
                    })
                .ToList();

            return View(prikaz);
        }

        [HttpGet]
        public IActionResult Create()
        {
            CiljStednje ciljStednje = new CiljStednje
            {
                DatumZavrsetka = DateTime.Today.AddMonths(6)
            };

            return View(ciljStednje);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Naziv,CiljaniIznos,DatumZavrsetka")]
            CiljStednje ciljStednje)
        {
            if (!ModelState.IsValid)
            {
                return View(ciljStednje);
            }

            string? korisnikId = _userManager.GetUserId(User);

            if (korisnikId == null)
            {
                return Challenge();
            }

            try
            {
                await _service.AddAsync(
                    ciljStednje,
                    korisnikId);

                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    exception.Message);

                return View(ciljStednje);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Uplata(int id)
        {
            string? korisnikId = _userManager.GetUserId(User);

            if (korisnikId == null)
            {
                return Challenge();
            }

            CiljStednje? cilj =
                await _service.GetByIdAsync(id, korisnikId);

            if (cilj == null)
            {
                return NotFound();
            }

            UplataNaCiljViewModel model =
                new UplataNaCiljViewModel
                {
                    Id = cilj.Id,
                    Naziv = cilj.Naziv,
                    CiljaniIznos = cilj.CiljaniIznos,
                    TrenutnoUstedjeno = cilj.TrenutnoUstedjeno
                };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Uplata(
    [Bind("Id,Iznos")]
    UplataNaCiljViewModel model)
        {
            string? korisnikId = _userManager.GetUserId(User);

            if (korisnikId == null)
            {
                return Challenge();
            }

            CiljStednje? cilj =
                await _service.GetByIdAsync(model.Id, korisnikId);

            if (cilj == null)
            {
                return NotFound();
            }

            model.Naziv = cilj.Naziv;
            model.CiljaniIznos = cilj.CiljaniIznos;
            model.TrenutnoUstedjeno = cilj.TrenutnoUstedjeno;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                bool uspjesno =
                    await _service.DodajUplatuAsync(
                        model.Id,
                        model.Iznos,
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
        public async Task<IActionResult> Edit(int id)
        {
            string? korisnikId = _userManager.GetUserId(User);

            if (korisnikId == null)
            {
                return Challenge();
            }

            CiljStednje? ciljStednje =
                await _service.GetByIdAsync(id, korisnikId);

            if (ciljStednje == null)
            {
                return NotFound();
            }

            return View(ciljStednje);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
    int id,
    [Bind("Id,Naziv,CiljaniIznos,DatumZavrsetka")]
    CiljStednje ciljStednje)
        {
            if (id != ciljStednje.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(ciljStednje);
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
                        ciljStednje,
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

                return View(ciljStednje);
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

            CiljStednje? ciljStednje =
                await _service.GetByIdAsync(id, korisnikId);

            if (ciljStednje == null)
            {
                return NotFound();
            }

            return View(ciljStednje);
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