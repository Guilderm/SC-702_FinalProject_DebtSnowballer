using Microsoft.EntityFrameworkCore;

namespace Server.DAL.Models;

public partial class DebtSnowballerDbContext : DbContext
{
	public DebtSnowballerDbContext()
	{
	}

	public DebtSnowballerDbContext(DbContextOptions<DebtSnowballerDbContext> options)
		: base(options)
	{
	}

	public virtual DbSet<Currency> Currencies { get; set; } = null!;
	public virtual DbSet<DebtPayDownMethod> DebtPayDownMethods { get; set; } = null!;
	public virtual DbSet<ExchangeRate> ExchangeRates { get; set; } = null!;
	public virtual DbSet<LoanDetail> LoanDetails { get; set; } = null!;
	public virtual DbSet<PlannedSnowflake> PlannedSnowflakes { get; set; } = null!;
	public virtual DbSet<SessionLog> SessionLogs { get; set; } = null!;
	public virtual DbSet<UserPreference> UserPreferences { get; set; } = null!;
	public virtual DbSet<UserProfile> UserProfiles { get; set; } = null!;
	public virtual DbSet<UserRole> UserRoles { get; set; } = null!;

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		if (!optionsBuilder.IsConfigured)
			optionsBuilder.UseSqlServer("Name=DefaultDBConnection");
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Currency>(entity =>
		{
			entity.HasIndex(e => e.AlphaCode, "UQ__Currenci__C0B0B3A2D0034EBB")
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
				.WithMany(p => p.ExchangeRateBaseCurrencyNavigations)
				.HasPrincipalKey(p => p.AlphaCode)
				.HasForeignKey(d => d.BaseCurrency)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK__ExchangeR__BaseC__088B3037");

			entity.HasOne(d => d.QuoteCurrencyNavigation)
				.WithMany(p => p.ExchangeRateQuoteCurrencyNavigations)
				.HasPrincipalKey(p => p.AlphaCode)
				.HasForeignKey(d => d.QuoteCurrency)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK__ExchangeR__Quote__0A7378A9");
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

			entity.Property(e => e.MonthlyInterestRate).HasColumnType("decimal(6, 4)");

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
				.HasConstraintName("FK__LoanDetai__Auth0__24334AAC");

			entity.HasOne(d => d.CurrencyCodeNavigation)
				.WithMany(p => p.LoanDetails)
				.HasPrincipalKey(p => p.AlphaCode)
				.HasForeignKey(d => d.CurrencyCode)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK__LoanDetai__Curre__2BD46C74");
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
				.HasConstraintName("FK__PlannedSn__Auth0__2FA4FD58");

			entity.HasOne(d => d.CurrencyCodeNavigation)
				.WithMany(p => p.PlannedSnowflakes)
				.HasPrincipalKey(p => p.AlphaCode)
				.HasForeignKey(d => d.CurrencyCode)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK__PlannedSn__Curre__355DD6AE");
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
				.HasConstraintName("FK__SessionLo__Auth0__2062B9C8");
		});

		modelBuilder.Entity<UserPreference>(entity =>
		{
			entity.ToTable("userPreferences");

			entity.Property(e => e.Auth0UserId).HasMaxLength(75);

			entity.Property(e => e.BaseCurrency)
				.HasMaxLength(3)
				.HasDefaultValueSql("('USD')");

			entity.Property(e => e.DebtPlanMonthlyPayment)
				.HasColumnType("decimal(18, 2)")
				.HasDefaultValueSql("((0.01))");

			entity.Property(e => e.SelectedStrategy).HasDefaultValueSql("((1))");

			entity.HasOne(d => d.Auth0User)
				.WithMany(p => p.UserPreferences)
				.HasPrincipalKey(p => p.Auth0UserId)
				.HasForeignKey(d => d.Auth0UserId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK__userPrefe__Auth0__17CD73C7");

			entity.HasOne(d => d.BaseCurrencyNavigation)
				.WithMany(p => p.UserPreferences)
				.HasPrincipalKey(p => p.AlphaCode)
				.HasForeignKey(d => d.BaseCurrency)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK__userPrefe__BaseC__19B5BC39");

			entity.HasOne(d => d.SelectedStrategyNavigation)
				.WithMany(p => p.UserPreferences)
				.HasForeignKey(d => d.SelectedStrategy)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK__userPrefe__Selec__1D864D1D");
		});

		modelBuilder.Entity<UserProfile>(entity =>
		{
			entity.ToTable("UserProfile");

			entity.HasIndex(e => e.Auth0UserId, "UQ__UserProf__1C8F429022176AD4")
				.IsUnique();

			entity.Property(e => e.Auth0UserId).HasMaxLength(75);

			entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

			entity.Property(e => e.Email).HasMaxLength(100);

			entity.Property(e => e.FamilyName).HasMaxLength(50);

			entity.Property(e => e.FullName).HasMaxLength(100);

			entity.Property(e => e.GivenName).HasMaxLength(50);

			entity.Property(e => e.LastUpdated).HasDefaultValueSql("(getdate())");

			entity.Property(e => e.Locale).HasMaxLength(10);

			entity.Property(e => e.NickName).HasMaxLength(50);

			entity.Property(e => e.Picture).HasMaxLength(300);

			entity.Property(e => e.UserRoleId).HasDefaultValueSql("((1))");

			entity.HasOne(d => d.UserRole)
				.WithMany(p => p.UserProfiles)
				.HasForeignKey(d => d.UserRoleId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK__UserProfi__UserR__1308BEAA");
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