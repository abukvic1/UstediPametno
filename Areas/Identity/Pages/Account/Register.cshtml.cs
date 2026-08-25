using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using UstediPametno.Models;

namespace UstediPametno.Areas.Identity.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly SignInManager<Korisnik> _signInManager;
    private readonly UserManager<Korisnik> _userManager;
    private readonly IUserStore<Korisnik> _userStore;
    private readonly IUserEmailStore<Korisnik> _emailStore;
    private readonly ILogger<RegisterModel> _logger;
    private readonly IEmailSender _emailSender;

    public RegisterModel(
        UserManager<Korisnik> userManager,
        IUserStore<Korisnik> userStore,
        SignInManager<Korisnik> signInManager,
        ILogger<RegisterModel> logger,
        IEmailSender emailSender)
    {
        _userManager = userManager;
        _userStore = userStore;
        _emailStore = GetEmailStore();
        _signInManager = signInManager;
        _logger = logger;
        _emailSender = emailSender;
    }

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public string? ReturnUrl { get; set; }

    public IList<AuthenticationScheme>? ExternalLogins { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Email je obavezan.")]
        [EmailAddress(ErrorMessage = "Email adresa nije ispravna.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "Lozinka je obavezna.")]
        [StringLength(
            100,
            ErrorMessage = "Lozinka mora imati najmanje {2}, a najviše {1} znakova.",
            MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Lozinka")]
        public string Password { get; set; } = default!;

        [DataType(DataType.Password)]
        [Display(Name = "Potvrdi lozinku")]
        [Compare(
            "Password",
            ErrorMessage = "Lozinka i potvrda lozinke se ne podudaraju.")]
        public string? ConfirmPassword { get; set; }
    }

    public async Task OnGetAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;

        ExternalLogins = (await _signInManager
            .GetExternalAuthenticationSchemesAsync())
            .ToList();
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        ExternalLogins = (await _signInManager
            .GetExternalAuthenticationSchemesAsync())
            .ToList();

        if (ModelState.IsValid)
        {
            var user = CreateUser();

            await _userStore.SetUserNameAsync(
                user,
                Input.Email,
                CancellationToken.None);

            await _emailStore.SetEmailAsync(
                user,
                Input.Email,
                CancellationToken.None);

            var result = await _userManager.CreateAsync(
                user,
                Input.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(
                    user,
                    "Korisnik");

                _logger.LogInformation(
                    "Korisnik je kreirao novi račun.");

                var userId = await _userManager.GetUserIdAsync(user);

                var code = await _userManager
                    .GenerateEmailConfirmationTokenAsync(user);

                code = WebEncoders.Base64UrlEncode(
                    Encoding.UTF8.GetBytes(code));

                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new
                    {
                        area = "Identity",
                        userId = userId,
                        code = code,
                        returnUrl = returnUrl
                    },
                    protocol: Request.Scheme)!;

                await _emailSender.SendEmailAsync(
                    Input.Email,
                    "Potvrdi email",
                    $"Potvrdi svoj račun klikom <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>ovdje</a>.");

                if (_userManager.Options.SignIn.RequireConfirmedAccount)
                {
                    return RedirectToPage(
                        "RegisterConfirmation",
                        new
                        {
                            email = Input.Email,
                            returnUrl = returnUrl
                        });
                }
                else
                {
                    await _signInManager.SignInAsync(
                        user,
                        isPersistent: false);

                    return LocalRedirect(returnUrl);
                }
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error.Description);
            }
        }

        return Page();
    }

    private Korisnik CreateUser()
    {
        try
        {
            return Activator.CreateInstance<Korisnik>();
        }
        catch
        {
            throw new InvalidOperationException(
                $"Nije moguće kreirati instancu klase '{nameof(Korisnik)}'.");
        }
    }

    private IUserEmailStore<Korisnik> GetEmailStore()
    {
        if (!_userManager.SupportsUserEmail)
        {
            throw new NotSupportedException(
                "Korisnički store mora podržavati email.");
        }

        return (IUserEmailStore<Korisnik>)_userStore;
    }
}