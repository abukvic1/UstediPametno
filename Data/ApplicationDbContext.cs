using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UstediPametno.Models;

namespace UstediPametno.Data
{
    public class ApplicationDbContext: IdentityDbContext<Korisnik>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options


            ) : base(options) { }
        public DbSet<Korisnik> Korisnik { get; set; }
        public DbSet<IzvorPrihoda> IzvorPrihoda { get; set; }
        public DbSet<FiksniTrosak> FiksniTrosak { get; set; }
        public DbSet<CiljStednje> CiljStednje { get; set; }
        public DbSet<MjesecniPlan> MjesecniPlan { get; set; }
        public DbSet<Transakcija> Transakcija { get; set; }
        public DbSet<MjesecniPlanCilj> MjesecniPlanCiljevi { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Korisnik>().ToTable("Korisnik");
            modelBuilder.Entity<IzvorPrihoda>().ToTable("IzvorPrihoda");
            modelBuilder.Entity<FiksniTrosak>().ToTable("FiksniTrosak");
            modelBuilder.Entity<CiljStednje>().ToTable("CiljStednje");
            modelBuilder.Entity<MjesecniPlan>().ToTable("MjesecniPlan");
            modelBuilder.Entity<Transakcija>().ToTable("Transakcija");
            modelBuilder.Entity<MjesecniPlanCilj>()
    .ToTable("MjesecniPlanCilj");
            modelBuilder.Entity<MjesecniPlanCilj>()
    .HasOne(mc => mc.MjesecniPlan)
    .WithMany(mp => mp.Ciljevi)
    .HasForeignKey(mc => mc.MjesecniPlanId)
    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MjesecniPlanCilj>()
                .HasOne(mc => mc.CiljStednje)
                .WithMany()
                .HasForeignKey(mc => mc.CiljStednjeId)
                .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
