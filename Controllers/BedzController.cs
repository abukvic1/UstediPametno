using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UstediPametno.Models;
using UstediPametno.Services;

namespace UstediPametno.Controllers
{
    [Authorize]
    public class BedzController : Controller
    {
        private readonly IBedzService _bedzService;
        private readonly UserManager<Korisnik> _userManager;

        public BedzController(
            IBedzService bedzService,
            UserManager<Korisnik> userManager)
        {
            _bedzService = bedzService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            Korisnik? korisnik =
                await _userManager.GetUserAsync(User);

            if (korisnik == null)
            {
                return Challenge();
            }

            await _bedzService.ProvjeriBedzeveAsync(korisnik.Id);

            var bedzevi =
                await _bedzService.GetBedzeveKorisnikaAsync(korisnik.Id);

            return View(bedzevi);
        }
    }
}