using Microsoft.EntityFrameworkCore;

namespace Server.DAL.Models;

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
	public virtual DbSet<DebtPayDownMethod> DebtPayDownMethods { get; set; } = null!;
	public virtual DbSet<ExchangeRate> ExchangeRates { get; set; } = null!;
	public virtual DbSet<LoanDetail> LoanDetails { get; set; } = null!;
	public virtual DbSet<PlannedSnowflake> PlannedSnowflakes { get; set; } = null!;
	public virtual DbSet<SessionLog> SessionLogs { get; set; } = null!;
	public virtual DbSet<UserProfile> UserProfiles { get; set; } = null!;
	public virtual DbSet<UserRole> UserRoles { get; set; } = null!;

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		if (!optionsBuilder.IsConfigured) optionsBuilder.UseSqlServer("Name=AzureBDConnection");
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Currency>(entity =>
		{
			entity.HasIndex(e => e.AlphaCode, "UQ__Currenci__C0B0B3A2678BA85B")
				.IsUnique();

			entity.Property(e => e.AlphaCode).HasMaxLength(3);

			entity.Property(e => e.Name).HasMaxLength(50);

			entity.Property(e => e.Symbol).HasMaxLength(5);
		});

		modelBuilder.Entity<DebtPayDownMethod>(entity =>
		{
			entity.ToTable("DebtPayDownMethod");

			entity.Property(e => e.Name).HasMaxLength(50);
		});

		modelBuilder.Entity<ExchangeRate>(entity =>
		{
			entity.ToTable("ExchangeRate");

			entity.Property(e => e.BaseCurrency)
				.HasMaxLength(3)
				.HasDefaultValueSql("('USD')");

			entity.Property(e => e.ConversionRate).HasColumnType("decimal(19, 9)");

			entity.Property(e => e.QuoteCurrency)
				.HasMaxLength(3)
				.HasDefaultValueSql("('USD')");

			entity.HasOne(d => d.BaseCurrencyNavigation)
				.WithMany(p => p.ExchangeRateBaseCurrencyNavigation)
				.HasPrincipalKey(p => p.AlphaCode)
				.HasForeignKey(d => d.BaseCurrency)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK__ExchangeR__BaseC__496FBC53");

			entity.HasOne(d => d.QuoteCurrencyNavigation)
				.WithMany(p => p.ExchangeRateQuoteCurrencyNavigation)
				.HasPrincipalKey(p => p.AlphaCode)
				.HasForeignKey(d => d.QuoteCurrency)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK__ExchangeR__Quote__4B5804C5");
		});

		modelBuilder.Entity<LoanDetail>(entity =>
		{
			entity.ToTable("LoanDetail");

			entity.Property(e => e.AnnualInterestRate).HasColumnType("decimal(6, 4)");

			entity.Property(e => e.Auth0UserId).HasMaxLength(75);

			entity.Property(e => e.BankFees).HasColumnType("decimal(18, 2)");

			entity.Property(e => e.ContractedMonthlyPayment).HasColumnType("decimal(18, 2)");

			entity.Property(e => e.CurrencyCode)
				.HasMaxLength(3)
				.HasDefaultValueSql("('USD')");

			entity.Property(e => e.Name).HasMaxLength(50);

			entity.Property(e => e.RemainingPrincipal).HasColumnType("decimal(18, 2)");

			entity.Property(e => e.StartDate)
				.HasColumnType("date")
				.HasDefaultValueSql("(datefromparts(datepart(year,getdate()),datepart(month,getdate()),(1)))");

			entity.HasOne(d => d.Auth0User)
				.WithMany(p => p.LoanDetails)
				.HasPrincipalKey(p => p.Auth0UserId)
				.HasForeignKey(d => d.Auth0UserId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK__LoanDetai__Auth0__6423B28F");

			entity.HasOne(d => d.CurrencyCodeNavigation)
				.WithMany(p => p.LoanDetails)
				.HasPrincipalKey(p => p.AlphaCode)
				.HasForeignKey(d => d.CurrencyCode)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK__LoanDetai__Curre__6AD0B01E");
		});

		modelBuilder.Entity<PlannedSnowflake>(entity =>
		{
			entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");

			entity.Property(e => e.Auth0UserId).HasMaxLength(75);

			entity.Property(e => e.CurrencyCode)
				.HasMaxLength(3)
				.HasDefaultValueSql("('USD')");

			entity.Property(e => e.EndingAt)
				.HasColumnType("date")
				.HasDefaultValueSql("(dateadd(year,(45),getdate()))");

			entity.Property(e => e.Name).HasMaxLength(50);

			entity.Property(e => e.StartingAt)
				.HasColumnType("date")
				.HasDefaultValueSql("(datefromparts(datepart(year,getdate()),datepart(month,getdate()),(1)))");

			entity.HasOne(d => d.Auth0User)
				.WithMany(p => p.PlannedSnowflakes)
				.HasPrincipalKey(p => p.Auth0UserId)
				.HasForeignKey(d => d.Auth0UserId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK__PlannedSn__Auth0__6EA14102");

			entity.HasOne(d => d.CurrencyCodeNavigation)
				.WithMany(p => p.PlannedSnowflakes)
				.HasPrincipalKey(p => p.AlphaCode)
				.HasForeignKey(d => d.CurrencyCode)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK__PlannedSn__Curre__745A1A58");
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
				.HasConstraintName("FK__SessionLo__Auth0__605321AB");
		});

		modelBuilder.Entity<UserProfile>(entity =>
		{
			entity.ToTable("UserProfile");

			entity.HasIndex(e => e.Auth0UserId, "UQ__UserProf__1C8F4290CB20846B")
				.IsUnique();

			entity.Property(e => e.AggregatedMonthlyPayment).HasColumnType("decimal(18, 2)");

			entity.Property(e => e.Auth0UserId).HasMaxLength(75);

			entity.Property(e => e.BaseCurrency)
				.HasMaxLength(3)
				.HasDefaultValueSql("('USD')");

			entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

			entity.Property(e => e.DebtPlanMonthlyPayment).HasColumnType("decimal(18, 2)");

			entity.Property(e => e.Email).HasMaxLength(256);

			entity.Property(e => e.FamilyName).HasMaxLength(50);

			entity.Property(e => e.FullName).HasMaxLength(100);

			entity.Property(e => e.GivenName).HasMaxLength(50);

			entity.Property(e => e.LastUpdated).HasDefaultValueSql("(getdate())");

			entity.Property(e => e.Locale).HasMaxLength(10);

			entity.Property(e => e.NickName).HasMaxLength(50);

			entity.Property(e => e.SelectedStrategy).HasDefaultValueSql("((1))");

			entity.Property(e => e.TotalAmountOwed).HasColumnType("decimal(18, 2)");

			entity.Property(e => e.UserRoleId).HasDefaultValueSql("((1))");

			entity.HasOne(d => d.SelectedStrategyNavigation)
				.WithMany(p => p.UserProfiles)
				.HasForeignKey(d => d.SelectedStrategy)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK__UserProfi__Selec__5A9A4855");

			entity.HasOne(d => d.UserRole)
				.WithMany(p => p.UserProfiles)
				.HasForeignKey(d => d.UserRoleId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK__UserProfi__UserR__53ED4AC6");
		});

		modelBuilder.Entity<UserRole>(entity =>
		{
			entity.ToTable("UserRole");

			entity.Property(e => e.Description).HasMaxLength(50);

			entity.Property(e => e.Name).HasMaxLength(20);
		});

		OnModelCreatingPartial(modelBuilder);
	}

	partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}