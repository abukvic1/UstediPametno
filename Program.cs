using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UstediPametno.Data;
using UstediPametno.Models;
using UstediPametno.Repositories;
using UstediPametno.Services;

var builder = WebApplication.CreateBuilder(args);

// ===============================
// BAZA
// ===============================

var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));


// ===============================
// REPOSITORY
// ===============================

builder.Services.AddScoped(
    typeof(IGenericRepository<>),
    typeof(GenericRepository<>));


// ===============================
// SERVICES
// ===============================

builder.Services.AddScoped<IIzvorPrihodaService, IzvorPrihodaService>();
builder.Services.AddScoped<IFiksniTrosakService, FiksniTrosakService>();
builder.Services.AddScoped<ICiljStednjeService, CiljStednjeService>();
builder.Services.AddScoped<IMjesecniPlanService, MjesecniPlanService>();
builder.Services.AddScoped<ITransakcijaService, TransakcijaService>();
builder.Services.AddScoped<
    IMjesecniPlanCiljService,
    MjesecniPlanCiljService>();


// ===============================
// IDENTITY
// ===============================

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services
    .AddDefaultIdentity<Korisnik>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();


// ===============================
// MVC
// ===============================

builder.Services.AddControllersWithViews();

var app = builder.Build();


// ===============================
// ROLE + ADMIN
// ===============================

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<IdentityRole>>();

    var userManager = scope.ServiceProvider
        .GetRequiredService<UserManager<Korisnik>>();

    string[] roles =
    {
        "Admin",
        "Korisnik"
    };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(
                new IdentityRole(role));
        }
    }


    // ===========================
    // ADMIN NALOG
    // ===========================

    const string adminEmail =
        "admin@ustedipametno.ba";

    const string adminPassword =
        "Admin123!";

    var admin = await userManager
        .FindByEmailAsync(adminEmail);

    if (admin == null)
    {
        admin = new Korisnik
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            ImePrezime = "Administrator"
        };

        var result = await userManager.CreateAsync(
            admin,
            adminPassword);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(
                admin,
                "Admin");
        }
    }
    else
    {
        if (!await userManager.IsInRoleAsync(
            admin,
            "Admin"))
        {
            await userManager.AddToRoleAsync(
                admin,
                "Admin");
        }
    }


    // ===========================
    // POSTOJEĆI KORISNICI
    // ===========================

    var sviKorisnici =
        await userManager.Users.ToListAsync();

    foreach (var korisnik in sviKorisnici)
    {
        if (korisnik.Email == adminEmail)
        {
            continue;
        }

        var jeAdmin =
            await userManager.IsInRoleAsync(
                korisnik,
                "Admin");

        var jeKorisnik =
            await userManager.IsInRoleAsync(
                korisnik,
                "Korisnik");

        if (!jeAdmin && !jeKorisnik)
        {
            await userManager.AddToRoleAsync(
                korisnik,
                "Korisnik");
        }
    }
}


// ===============================
// HTTP PIPELINE
// ===============================

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();


// Identity koristi authentication cookie.
// UseAuthentication ide prije UseAuthorization.
app.UseAuthentication();
app.UseAuthorization();


// ===============================
// STATIC FILES
// ===============================

app.MapStaticAssets();


// ===============================
// ROUTES
// ===============================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
    .WithStaticAssets();

app.Run();