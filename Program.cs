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
builder.Services.AddScoped<IMjesecniPlanCiljService, MjesecniPlanCiljService>();

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