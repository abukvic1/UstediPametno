using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using UstediPametno.Models;
using UstediPametno.Repositories;
using UstediPametno.ViewModels;
namespace UstediPametno.Services
{
    public class BedzService : IBedzService
    {
        private readonly IGenericRepository<Bedz> _bedzRepository;
        private readonly IGenericRepository<KorisnikBedz> _korisnikBedzRepository;
        private readonly IGenericRepository<Transakcija> _transakcijaRepository;
        private readonly UserManager<Korisnik> _userManager;
        private readonly IEmailService _emailService;
        private readonly ILogger<BedzService> _logger;

        public BedzService(
            IGenericRepository<Bedz> bedzRepository,
            IGenericRepository<KorisnikBedz> korisnikBedzRepository,
            IGenericRepository<Transakcija> transakcijaRepository,
            UserManager<Korisnik> userManager,
            IEmailService emailService,
            ILogger<BedzService> logger)
        {
            _bedzRepository = bedzRepository;
            _korisnikBedzRepository = korisnikBedzRepository;
            _transakcijaRepository = transakcijaRepository;
            _userManager = userManager;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task ProvjeriBedzeveAsync(string korisnikId)
        {
            Korisnik? korisnik =
                await _userManager.FindByIdAsync(korisnikId);

            if (korisnik == null)
            {
                return;
            }

            var transakcije =
                await _transakcijaRepository.GetAllAsync();

            decimal ukupnoUstedjeno = transakcije
                .Where(t =>
                    t.KorisnikId == korisnikId &&
                    t.Vrsta == VrstaTransakcije.UplataStednje)
                .Sum(t => t.Iznos);

            var sviBedzevi =
                await _bedzRepository.GetAllAsync();

            var osvojeniBedzevi =
                await _korisnikBedzRepository.GetAllAsync();

            var osvojeniBedzIds = osvojeniBedzevi
                .Where(kb => kb.KorisnikId == korisnikId)
                .Select(kb => kb.BedzId)
                .ToHashSet();

            // =========================
            // BEDZEVI ZA STEDNJU
            // =========================

            var stednjaBedzevi = sviBedzevi
                .Where(b =>
                    b.Vrsta == VrstaBedza.Stednja &&
                    ukupnoUstedjeno >= b.Prag &&
                    !osvojeniBedzIds.Contains(b.Id))
                .ToList();

            foreach (var bedz in stednjaBedzevi)
            {
                await DodijeliBedzAsync(
                    korisnik,
                    bedz,
                    osvojeniBedzIds);
            }

            // =========================
            // LOYALTY BEDZEVI
            // =========================

            DateTime danas = DateTime.UtcNow;

            int brojMjeseci =
                (danas.Year - korisnik.DatumKreiranja.Year) * 12
                + danas.Month
                - korisnik.DatumKreiranja.Month;

            if (danas.Day < korisnik.DatumKreiranja.Day)
            {
                brojMjeseci--;
            }

            brojMjeseci = Math.Max(0, brojMjeseci);

            var loyaltyBedzevi = sviBedzevi
                .Where(b =>
                    b.Vrsta == VrstaBedza.Lojalnost &&
                    brojMjeseci >= b.Prag &&
                    !osvojeniBedzIds.Contains(b.Id))
                .ToList();

            foreach (var bedz in loyaltyBedzevi)
            {
                await DodijeliBedzAsync(
                    korisnik,
                    bedz,
                    osvojeniBedzIds);
            }
        }

        private async Task DodijeliBedzAsync(
            Korisnik korisnik,
            Bedz bedz,
            HashSet<int> osvojeniBedzIds)
        {
            var korisnikBedz = new KorisnikBedz
            {
                KorisnikId = korisnik.Id,
                BedzId = bedz.Id,
                DatumOsvajanja = DateTime.UtcNow
            };

            await _korisnikBedzRepository.AddAsync(korisnikBedz);
            await _korisnikBedzRepository.SaveChangesAsync();

            osvojeniBedzIds.Add(bedz.Id);

            if (string.IsNullOrWhiteSpace(korisnik.Email))
            {
                return;
            }

            try
            {
                string naslov =
                    $"🎉 Osvojili ste bedž: {bedz.Naziv}";

                string poruka = $@"
                    <h2>Čestitamo, {korisnik.ImePrezime}! 🎉</h2>

                    <p>Upravo ste osvojili novi bedž u aplikaciji
                    <strong>Uštedi Pametno</strong>.</p>

                    <h3>{bedz.Ikona} {bedz.Naziv}</h3>

                    <p>{bedz.Opis}</p>

                    <p>Nastavite pametno štedjeti! 💰</p>
                ";

                await _emailService.PosaljiAsync(
                    korisnik.Email,
                    naslov,
                    poruka);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Greška pri slanju emaila za bedž {BedzNaziv} korisniku {KorisnikId}.",
                    bedz.Naziv,
                    korisnik.Id);
            }
        }

        public async Task<IEnumerable<BedzPrikazViewModel>>
     GetBedzeveKorisnikaAsync(string korisnikId)
        {
            Korisnik? korisnik =
                await _userManager.FindByIdAsync(korisnikId);

            if (korisnik == null)
            {
                return new List<BedzPrikazViewModel>();
            }

            var sviBedzevi =
                await _bedzRepository.GetAllAsync();

            var korisnikBedzevi =
                await _korisnikBedzRepository.GetAllAsync();

            var transakcije =
                await _transakcijaRepository.GetAllAsync();

            decimal ukupnoUstedjeno = transakcije
                .Where(t =>
                    t.KorisnikId == korisnikId &&
                    t.Vrsta == VrstaTransakcije.UplataStednje)
                .Sum(t => t.Iznos);

            DateTime danas = DateTime.UtcNow;

            int brojMjeseci =
                (danas.Year - korisnik.DatumKreiranja.Year) * 12
                + danas.Month
                - korisnik.DatumKreiranja.Month;

            if (danas.Day < korisnik.DatumKreiranja.Day)
            {
                brojMjeseci--;
            }

            brojMjeseci = Math.Max(0, brojMjeseci);

            var osvojeni = korisnikBedzevi
                .Where(kb => kb.KorisnikId == korisnikId)
                .ToDictionary(
                    kb => kb.BedzId,
                    kb => kb.DatumOsvajanja);

            var rezultat = new List<BedzPrikazViewModel>();

            foreach (var bedz in sviBedzevi)
            {
                decimal trenutnaVrijednost;

                if (bedz.Vrsta == VrstaBedza.Stednja)
                {
                    trenutnaVrijednost = ukupnoUstedjeno;
                }
                else
                {
                    trenutnaVrijednost = brojMjeseci;
                }

                decimal procenat = bedz.Prag > 0
                    ? (trenutnaVrijednost / bedz.Prag) * 100
                    : 0;

                procenat = Math.Min(100, procenat);

                rezultat.Add(new BedzPrikazViewModel
                {
                    Id = bedz.Id,
                    Naziv = bedz.Naziv,
                    Opis = bedz.Opis,
                    Ikona = bedz.Ikona,
                    Vrsta = bedz.Vrsta,
                    Prag = bedz.Prag,

                    Osvojen = osvojeni.ContainsKey(bedz.Id),

                    DatumOsvajanja =
                        osvojeni.ContainsKey(bedz.Id)
                            ? osvojeni[bedz.Id]
                            : null,

                    TrenutnaVrijednost = trenutnaVrijednost,

                    NapredakProcenat = procenat
                });
            }

            return rezultat
                .OrderBy(b => b.Vrsta)
                .ThenBy(b => b.Prag)
                .ToList();
        }
    }
}