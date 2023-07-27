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

        public virtual DbSet<Crud> Cruds { get; set; } = null!;
        public virtual DbSet<Debt> Debts { get; set; } = null!;
        public virtual DbSet<DebtStrategy> DebtStrategies { get; set; } = null!;
        public virtual DbSet<MonthlyExtraPayment> MonthlyExtraPayments { get; set; } = null!;
        public virtual DbSet<OnetimeExtraPayment> OnetimeExtraPayments { get; set; } = null!;
        public virtual DbSet<SessionLog> SessionLogs { get; set; } = null!;
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
            modelBuilder.Entity<Crud>(entity =>
            {
                entity.ToTable("CRUD");

                entity.Property(e => e.InterestRate).HasColumnType("decimal(5, 2)");

                entity.Property(e => e.LoanName).HasMaxLength(50);

                entity.Property(e => e.Principal).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<Debt>(entity =>
            {
                entity.ToTable("Debt");

                entity.Property(e => e.Auth0UserId).HasMaxLength(125);

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CurrencyCode)
                    .HasMaxLength(3)
                    .HasDefaultValueSql("('USD')");

                entity.Property(e => e.Fees).HasColumnType("decimal(10, 3)");

                entity.Property(e => e.InterestRate).HasColumnType("decimal(5, 5)");

                entity.Property(e => e.LoanNickName).HasMaxLength(50);

                entity.Property(e => e.MonthlyPayment).HasColumnType("decimal(10, 3)");

                entity.Property(e => e.Principal).HasColumnType("decimal(10, 3)");
            });

            modelBuilder.Entity<DebtStrategy>(entity =>
            {
                entity.ToTable("DebtStrategy");

                entity.Property(e => e.Auth0UserId).HasMaxLength(125);

                entity.HasOne(d => d.Strategy)
                    .WithMany(p => p.DebtStrategies)
                    .HasForeignKey(d => d.StrategyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__DebtStrat__Strat__7ADC2F5E");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.DebtStrategies)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__DebtStrat__UserI__79E80B25");
            });

            modelBuilder.Entity<MonthlyExtraPayment>(entity =>
            {
                entity.Property(e => e.Amount).HasColumnType("decimal(10, 3)");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.MonthlyExtraPayments)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__MonthlyEx__UserI__01892CED");
            });

            modelBuilder.Entity<OnetimeExtraPayment>(entity =>
            {
                entity.Property(e => e.Amount).HasColumnType("decimal(10, 3)");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.OnetimeExtraPayments)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__OnetimeEx__UserI__04659998");
            });

            modelBuilder.Entity<SessionLog>(entity =>
            {
                entity.ToTable("SessionLog");

                entity.Property(e => e.ClientSoftware).HasMaxLength(50);

                entity.Property(e => e.LogonTimeStamp).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.OperatingSystem).HasMaxLength(50);

                entity.Property(e => e.RemoteIpAddress).HasMaxLength(50);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.SessionLogs)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__SessionLo__UserI__742F31CF");
            });

            modelBuilder.Entity<StrategyType>(entity =>
            {
                entity.ToTable("StrategyType");

                entity.Property(e => e.Type).HasMaxLength(50);
            });

            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.ToTable("UserProfile");

                entity.HasIndex(e => e.Auth0UserId, "UQ__UserProf__1C8F42903FA2A9A0")
                    .IsUnique();

                entity.Property(e => e.Auth0UserId).HasMaxLength(125);

                entity.Property(e => e.BaseCurrency)
                    .HasMaxLength(3)
                    .HasDefaultValueSql("('USD')");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.LastName).HasMaxLength(50);

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
