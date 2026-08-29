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
        public DbSet<Bedz> Bedz { get; set; }
        public DbSet<KorisnikBedz> KorisnikBedz { get; set; }
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
            modelBuilder.Entity<Bedz>()
    .ToTable("Bedz");

            modelBuilder.Entity<KorisnikBedz>()
                .ToTable("KorisnikBedz");

            modelBuilder.Entity<KorisnikBedz>()
                .HasOne(kb => kb.Korisnik)
                .WithMany()
                .HasForeignKey(kb => kb.KorisnikId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<KorisnikBedz>()
                .HasOne(kb => kb.Bedz)
                .WithMany()
                .HasForeignKey(kb => kb.BedzId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<KorisnikBedz>()
                .HasIndex(kb => new
                {
                    kb.KorisnikId,
                    kb.BedzId
                })
                .IsUnique();
            modelBuilder.Entity<Bedz>()
    .Property(b => b.Prag)
    .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Bedz>().HasData(
    new Bedz
    {
        Id = 1,
        Naziv = "Prvi korak",
        Opis = "Ušteđeno prvih 100 KM",
        Vrsta = VrstaBedza.Stednja,
        Prag = 100,
        Ikona = "🌱"
    },
    new Bedz
    {
        Id = 2,
        Naziv = "Drveni štediša",
        Opis = "Ušteđeno 500 KM",
        Vrsta = VrstaBedza.Stednja,
        Prag = 500,
        Ikona = "🪵"
    },
    new Bedz
    {
        Id = 3,
        Naziv = "Bronzani štediša",
        Opis = "Ušteđeno 1.000 KM",
        Vrsta = VrstaBedza.Stednja,
        Prag = 1000,
        Ikona = "🥉"
    },
    new Bedz
    {
        Id = 4,
        Naziv = "Srebreni štediša",
        Opis = "Ušteđeno 2.500 KM",
        Vrsta = VrstaBedza.Stednja,
        Prag = 2500,
        Ikona = "🥈"
    },
    new Bedz
    {
        Id = 5,
        Naziv = "Zlatni štediša",
        Opis = "Ušteđeno 5.000 KM",
        Vrsta = VrstaBedza.Stednja,
        Prag = 5000,
        Ikona = "🥇"
    },
    new Bedz
    {
        Id = 6,
        Naziv = "Dijamantni štediša",
        Opis = "Ušteđeno 10.000 KM",
        Vrsta = VrstaBedza.Stednja,
        Prag = 10000,
        Ikona = "💎"
    },

    new Bedz
    {
        Id = 7,
        Naziv = "Mjesec dana s nama",
        Opis = "Član aplikacije najmanje 1 mjesec",
        Vrsta = VrstaBedza.Lojalnost,
        Prag = 1,
        Ikona = "🌱"
    },
    new Bedz
    {
        Id = 8,
        Naziv = "Vjerni član",
        Opis = "Član aplikacije najmanje 3 mjeseca",
        Vrsta = VrstaBedza.Lojalnost,
        Prag = 3,
        Ikona = "🤝"
    },
    new Bedz
    {
        Id = 9,
        Naziv = "Pola godine s nama",
        Opis = "Član aplikacije najmanje 6 mjeseci",
        Vrsta = VrstaBedza.Lojalnost,
        Prag = 6,
        Ikona = "⭐"
    },
    new Bedz
    {
        Id = 10,
        Naziv = "Godinu dana s nama",
        Opis = "Član aplikacije najmanje 12 mjeseci",
        Vrsta = VrstaBedza.Lojalnost,
        Prag = 12,
        Ikona = "🎂"
    }
);

        }
    }
}
