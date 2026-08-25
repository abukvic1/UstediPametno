using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UstediPametno.Models;
using UstediPametno.Services;

namespace UstediPametno.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<Korisnik> _userManager;
        private readonly ITransakcijaService _transakcijaService;
        private readonly ICiljStednjeService _ciljStednjeService;
        private readonly IMjesecniPlanService _mjesecniPlanService;

        public AdminController(
            UserManager<Korisnik> userManager,
            ITransakcijaService transakcijaService,
            ICiljStednjeService ciljStednjeService,
            IMjesecniPlanService mjesecniPlanService)
        {
            _userManager = userManager;
            _transakcijaService = transakcijaService;
            _ciljStednjeService = ciljStednjeService;
            _mjesecniPlanService = mjesecniPlanService;
        }

        public async Task<IActionResult> Index()
        {
            var korisnici = await _userManager.Users
                .Where(k => k.Email != "admin@ustedipametno.ba")
                .ToListAsync();

            int brojKorisnika = korisnici.Count;
            int brojTransakcija = 0;
            int brojCiljeva = 0;
            int brojPlanova = 0;

            foreach (var korisnik in korisnici)
            {
                var transakcije =
                    await _transakcijaService.GetAllAsync(korisnik.Id);

                var ciljevi =
                    await _ciljStednjeService.GetAllAsync(korisnik.Id);

                var planovi =
                    await _mjesecniPlanService.GetAllAsync(korisnik.Id);

                brojTransakcija += transakcije.Count();
                brojCiljeva += ciljevi.Count();
                brojPlanova += planovi.Count();
            }

            ViewBag.BrojKorisnika = brojKorisnika;
            ViewBag.BrojTransakcija = brojTransakcija;
            ViewBag.BrojCiljeva = brojCiljeva;
            ViewBag.BrojPlanova = brojPlanova;

            return View();
        }

        public async Task<IActionResult> Korisnici()
        {
            var korisnici = await _userManager.Users
                .Where(k => k.Email != "admin@ustedipametno.ba")
                .OrderByDescending(k => k.DatumKreiranja)
                .ToListAsync();

            return View(korisnici);
        }
    }
}