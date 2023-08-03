using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Server.DAL.Models
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

        public virtual DbSet<Debt> Debts { get; set; } = null!;
        public virtual DbSet<DebtStrategy> DebtStrategies { get; set; } = null!;
        public virtual DbSet<ExchangeRate> ExchangeRates { get; set; } = null!;
        public virtual DbSet<SessionLog> SessionLogs { get; set; } = null!;
        public virtual DbSet<Snowflake> Snowflakes { get; set; } = null!;
        public virtual DbSet<StrategyType> StrategyTypes { get; set; } = null!;
        public virtual DbSet<UserProfile> UserProfiles { get; set; } = null!;
        public virtual DbSet<UserType> UserTypes { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=AzureBDConnection");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Debt>(entity =>
            {
                entity.ToTable("Debt");

                entity.Property(e => e.Auth0UserId).HasMaxLength(75);

                entity.Property(e => e.BankFees).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CurrencyCode)
                    .HasMaxLength(3)
                    .HasDefaultValueSql("('USD')");

                entity.Property(e => e.InterestRate).HasColumnType("decimal(6, 4)");

                entity.Property(e => e.LoanNickName).HasMaxLength(50);

                entity.Property(e => e.MonthlyPayment).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.RemainingPrincipal).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Auth0User)
                    .WithMany(p => p.Debts)
                    .HasPrincipalKey(p => p.Auth0UserId)
                    .HasForeignKey(d => d.Auth0UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Debt__Auth0UserI__114071C9");
            });

            modelBuilder.Entity<DebtStrategy>(entity =>
            {
                entity.ToTable("DebtStrategy");

                entity.Property(e => e.Auth0UserId).HasMaxLength(75);

                entity.HasOne(d => d.Auth0User)
                    .WithMany(p => p.DebtStrategies)
                    .HasPrincipalKey(p => p.Auth0UserId)
                    .HasForeignKey(d => d.Auth0UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__DebtStrat__Auth0__0D6FE0E5");

                entity.HasOne(d => d.Strategy)
                    .WithMany(p => p.DebtStrategies)
                    .HasForeignKey(d => d.StrategyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__DebtStrat__Strat__0E64051E");
            });

            modelBuilder.Entity<ExchangeRate>(entity =>
            {
                entity.Property(e => e.BaseCurrency)
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.ConversionRate).HasColumnType("decimal(19, 9)");

                entity.Property(e => e.QuoteCurrency)
                    .HasMaxLength(3)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SessionLog>(entity =>
            {
                entity.ToTable("SessionLog");

                entity.Property(e => e.Auth0UserId).HasMaxLength(75);

                entity.Property(e => e.ClientSoftware).HasMaxLength(50);

                entity.Property(e => e.LogonTimeStamp).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.OperatingSystem).HasMaxLength(50);

                entity.Property(e => e.RemoteIpAddress).HasMaxLength(50);

                entity.HasOne(d => d.Auth0User)
                    .WithMany(p => p.SessionLogs)
                    .HasPrincipalKey(p => p.Auth0UserId)
                    .HasForeignKey(d => d.Auth0UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__SessionLo__Auth0__07B7078F");
            });

            modelBuilder.Entity<Snowflake>(entity =>
            {
                entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Auth0UserId).HasMaxLength(75);

                entity.HasOne(d => d.Auth0User)
                    .WithMany(p => p.Snowflakes)
                    .HasPrincipalKey(p => p.Auth0UserId)
                    .HasForeignKey(d => d.Auth0UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Snowflake__Auth0__160526E6");
            });

            modelBuilder.Entity<StrategyType>(entity =>
            {
                entity.ToTable("StrategyType");

                entity.Property(e => e.Type).HasMaxLength(50);
            });

            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.ToTable("UserProfile");

                entity.HasIndex(e => e.Auth0UserId, "UQ__UserProf__1C8F42905B87FBD6")
                    .IsUnique();

                entity.Property(e => e.Auth0UserId).HasMaxLength(75);

                entity.Property(e => e.BaseCurrency)
                    .HasMaxLength(3)
                    .HasDefaultValueSql("('USD')");

                entity.Property(e => e.ContractedMonthlyPayment)
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.FamilyName).HasMaxLength(50);

                entity.Property(e => e.FullName).HasMaxLength(100);

                entity.Property(e => e.GivenName).HasMaxLength(50);

                entity.Property(e => e.LastUpdated).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Locale).HasMaxLength(10);

                entity.Property(e => e.NickName).HasMaxLength(50);

                entity.Property(e => e.Picture).HasMaxLength(300);

                entity.Property(e => e.PreferredMonthlyPayment)
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.TotalAmountOwed)
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.UserTypeId).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<UserType>(entity =>
            {
                entity.ToTable("UserType");

                entity.Property(e => e.Description).HasMaxLength(50);

                entity.Property(e => e.Type).HasMaxLength(20);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
