using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DebtSnowballer.Server.Models
{
    public partial class DebtSnowballerContext : DbContext
    {
        public DebtSnowballerContext()
        {
        }

        public DebtSnowballerContext(DbContextOptions<DebtSnowballerContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AppUser> AppUsers { get; set; }
        public virtual DbSet<Crud> Cruds { get; set; }
        public virtual DbSet<Currency> Currencies { get; set; }
        public virtual DbSet<Debt> Debts { get; set; }
        public virtual DbSet<MonthlyExtraPayment> MonthlyExtraPayments { get; set; }
        public virtual DbSet<OnetimeExtraPayment> OnetimeExtraPayments { get; set; }
        public virtual DbSet<SessionLog> SessionLogs { get; set; }
        public virtual DbSet<UserType> UserTypes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=AzureBDConnection");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.ToTable("AppUser");

                entity.HasIndex(e => e.Auth0UserId, "UQ__AppUser__1C8F4290CAF88E53")
                    .IsUnique();

                entity.Property(e => e.Auth0UserId)
                    .IsRequired()
                    .HasMaxLength(125);

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UserTypeId).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<Crud>(entity =>
            {
                entity.ToTable("CRUD");

                entity.Property(e => e.InterestRate).HasColumnType("decimal(5, 2)");

                entity.Property(e => e.LoanName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Principal).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<Currency>(entity =>
            {
                entity.ToTable("Currency");

                entity.Property(e => e.FormalName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ShortName)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Symbol)
                    .IsRequired()
                    .HasMaxLength(10);
            });

            modelBuilder.Entity<Debt>(entity =>
            {
                entity.ToTable("Debt");

                entity.Property(e => e.Auth0UserId)
                    .IsRequired()
                    .HasMaxLength(125);

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CurrencyId)
                    .HasColumnName("CurrencyID")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Fees).HasColumnType("decimal(10, 3)");

                entity.Property(e => e.InterestRate).HasColumnType("decimal(5, 5)");

                entity.Property(e => e.LoanNickName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.MonthlyPayment).HasColumnType("decimal(10, 3)");

                entity.Property(e => e.Principal).HasColumnType("decimal(10, 3)");

                entity.HasOne(d => d.Currency)
                    .WithMany(p => p.Debts)
                    .HasForeignKey(d => d.CurrencyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Debt__CurrencyID__125EB334");
            });

            modelBuilder.Entity<MonthlyExtraPayment>(entity =>
            {
                entity.Property(e => e.Amount).HasColumnType("decimal(10, 3)");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.MonthlyExtraPayments)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__MonthlyEx__UserI__17236851");
            });

            modelBuilder.Entity<OnetimeExtraPayment>(entity =>
            {
                entity.Property(e => e.Amount).HasColumnType("decimal(10, 3)");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.OnetimeExtraPayments)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__OnetimeEx__UserI__19FFD4FC");
            });

            modelBuilder.Entity<SessionLog>(entity =>
            {
                entity.ToTable("SessionLog");

                entity.Property(e => e.ClientSoftware)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.LogonTimeStamp).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.OperatingSystem)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.RemoteIpAddress)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.SessionLogs)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__SessionLo__UserI__0CA5D9DE");
            });

            modelBuilder.Entity<UserType>(entity =>
            {
                entity.ToTable("UserType");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(20);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
