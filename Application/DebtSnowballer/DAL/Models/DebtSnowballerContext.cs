using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DAL.Models
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

        public virtual DbSet<Currency> Currencies { get; set; } = null!;
        public virtual DbSet<DebtSnowflake> DebtSnowflakes { get; set; } = null!;
        public virtual DbSet<Loan> Loans { get; set; } = null!;
        public virtual DbSet<LoanCardinalOrder> LoanCardinalOrders { get; set; } = null!;
        public virtual DbSet<PaymentStrategyPlan> PaymentStrategyPlans { get; set; } = null!;
        public virtual DbSet<SessionLog> SessionLogs { get; set; } = null!;
        public virtual DbSet<StrategyType> StrategyTypes { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
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
            modelBuilder.Entity<Currency>(entity =>
            {
                entity.ToTable("Currency");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FormalName).HasMaxLength(50);

                entity.Property(e => e.ShortName).HasMaxLength(20);

                entity.Property(e => e.Symbol).HasMaxLength(10);
            });

            modelBuilder.Entity<DebtSnowflake>(entity =>
            {
                entity.ToTable("DebtSnowflake");

                entity.Property(e => e.Amount).HasColumnType("decimal(10, 3)");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Date).HasColumnType("date");

                entity.HasOne(d => d.PaymentStrategyNavigation)
                    .WithMany(p => p.DebtSnowflakes)
                    .HasForeignKey(d => d.PaymentStrategy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DebtSnowflake_PaymentStrategy_PaymentStrategyPlan_Id");
            });

            modelBuilder.Entity<Loan>(entity =>
            {
                entity.ToTable("Loan");

                entity.HasIndex(e => e.LoanNickName, "UC_Loan_LoanNickName")
                    .IsUnique();

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Fees).HasColumnType("decimal(10, 3)");

                entity.Property(e => e.InterestRate).HasColumnType("decimal(5, 5)");

                entity.Property(e => e.LoanNickName).HasMaxLength(50);

                entity.Property(e => e.MonthlyPayment).HasColumnType("decimal(10, 3)");

                entity.Property(e => e.Principal).HasColumnType("decimal(10, 3)");

                entity.HasOne(d => d.CurrencyNavigation)
                    .WithMany(p => p.Loans)
                    .HasForeignKey(d => d.Currency)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Loan_Currency_Currency_Id");

                entity.HasOne(d => d.PaymentStrategyNavigation)
                    .WithMany(p => p.Loans)
                    .HasForeignKey(d => d.PaymentStrategy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Loan_PaymentStrategy_PaymentStrategyPlan_Id");
            });

            modelBuilder.Entity<LoanCardinalOrder>(entity =>
            {
                entity.ToTable("LoanCardinalOrder");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Loan)
                    .WithMany(p => p.LoanCardinalOrders)
                    .HasForeignKey(d => d.LoanId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LoanCardinalOrder_LoanId_Loan_Id");

                entity.HasOne(d => d.PaymentStrategyNavigation)
                    .WithMany(p => p.LoanCardinalOrders)
                    .HasForeignKey(d => d.PaymentStrategy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LoanCardinalOrder_PaymentStrategy_PaymentStrategyPlan_Id");
            });

            modelBuilder.Entity<PaymentStrategyPlan>(entity =>
            {
                entity.ToTable("PaymentStrategyPlan");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.GlobalMonthlyPayment).HasColumnType("decimal(10, 3)");

                entity.HasOne(d => d.StrategyType)
                    .WithMany(p => p.PaymentStrategyPlans)
                    .HasForeignKey(d => d.StrategyTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PaymentStrategyPlan_StrategyTypeId_StrategyType_Id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.PaymentStrategyPlans)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PaymentStrategyPlan_UserId_User_Id");
            });

            modelBuilder.Entity<SessionLog>(entity =>
            {
                entity.ToTable("SessionLog");

                entity.Property(e => e.ClientSoftware).HasMaxLength(50);

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.OperatingSystem).HasMaxLength(50);

                entity.Property(e => e.RemoteIpAddress).HasMaxLength(50);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.SessionLogs)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SessionLog_UserId_User_Id");
            });

            modelBuilder.Entity<StrategyType>(entity =>
            {
                entity.ToTable("StrategyType");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Type).HasMaxLength(20);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.HasIndex(e => e.Email, "UC_User_Email")
                    .IsUnique();

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.Password).HasMaxLength(255);

                entity.HasOne(d => d.UserType)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.UserTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_UserTypeId_UserType_Id");
            });

            modelBuilder.Entity<UserType>(entity =>
            {
                entity.ToTable("UserType");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Type).HasMaxLength(20);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
