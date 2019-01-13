using System;
using Microsoft.EntityFrameworkCore;
using ShipRegistryApplication;

namespace ShipRegistryPorts.Db
{
    public class ShipRegistryDbContext : DbContext
    {
        public DbSet<Ship> Ships { get; set; }
        public DbSet<ShippingLine> Lines { get; set; }

        public ShipRegistryDbContext(DbContextOptions<ShipRegistryDbContext> options)
            :base(options){}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFProviders.InMemory;Trusted_Connection=True;");
            }

            // Fixes issue with MySql connector reporting nested transactions not supported https://github.com/aspnet/EntityFrameworkCore/issues/7017
            Database.AutoTransactionsEnabled = false;

            base.OnConfiguring(optionsBuilder);
 
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ship>()
                .Property(s => s.Id)
                .HasColumnName("id")
                .HasConversion(i => i.Value, v => new Id(v))
                .IsRequired();

            modelBuilder.Entity<Ship>()
                .HasKey(field => field.Id);

            modelBuilder.Entity<Ship>()
                .Property(s => s.Capacity)
                .HasColumnName("Capacity")
                .HasConversion(c => c.Value, v => new Capacity(v))
                .IsRequired();

            modelBuilder.Entity<Ship>()
                .Property(s => s.ShipType)
                .HasColumnName("Type")
                .HasConversion(t => t.ToString(), v => (ShipType) Enum.Parse<ShipType>(v))
                .IsRequired();

            modelBuilder.Entity<Ship>()
                .Property(s => s.ShipName)
                .HasColumnName("Name")
                .HasConversion(n => n.ToString(), v => new ShipName(v))
                .IsRequired();

            modelBuilder.Entity<Ship>()
                .Property(s => s.ShippingLineId)
                .HasColumnName("LineId")
                .HasConversion(sli => sli.Value, v => new Id(v))
                .IsRequired();

            modelBuilder.Entity<Ship>()
                .Property(s => s.Version)
                .HasColumnName("Version")
                .IsRequired();
                
            modelBuilder.Entity<ShippingLine>()
                .Property(sl => sl.Id)
                .HasColumnName("Id")
                .HasConversion(i => i.Value, v => new Id(v))
                .IsRequired();

            modelBuilder.Entity<ShippingLine>()
                .Property(sl => sl.LineName)
                .HasColumnName("Name")
                .HasConversion(n => n.ToString(), v => new LineName(v))
                .IsRequired();
            
            modelBuilder.Entity<ShippingLine>()
                .HasKey(field => field.Id);
        }
     }
}