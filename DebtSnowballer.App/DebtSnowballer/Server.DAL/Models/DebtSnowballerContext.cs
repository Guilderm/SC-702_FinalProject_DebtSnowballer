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
	public virtual DbSet<Debt> Debts { get; set; } = null!;
	public virtual DbSet<ExchangeRate> ExchangeRates { get; set; } = null!;
	public virtual DbSet<SessionLog> SessionLogs { get; set; } = null!;
	public virtual DbSet<Snowflake> Snowflakes { get; set; } = null!;
	public virtual DbSet<StrategyType> StrategyTypes { get; set; } = null!;
	public virtual DbSet<UserProfile> UserProfiles { get; set; } = null!;
	public virtual DbSet<UserType> UserTypes { get; set; } = null!;

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		if (!optionsBuilder.IsConfigured) optionsBuilder.UseSqlServer("Name=AzureBDConnection");
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Currency>(entity =>
		{
			entity.HasIndex(e => e.AlphaCode, "UQ__Currenci__C0B0B3A2A62B7F46")
				.IsUnique();

			entity.Property(e => e.AlphaCode).HasMaxLength(3);

			entity.Property(e => e.Name).HasMaxLength(50);

			entity.Property(e => e.Symbol).HasMaxLength(5);
		});

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

			entity.Property(e => e.MonthlyPayment).HasColumnType("decimal(18, 2)");

			entity.Property(e => e.NickName).HasMaxLength(50);

			entity.Property(e => e.RemainingPrincipal).HasColumnType("decimal(18, 2)");

			entity.HasOne(d => d.Auth0User)
				.WithMany(p => p.Debts)
				.HasPrincipalKey(p => p.Auth0UserId)
				.HasForeignKey(d => d.Auth0UserId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK__Debt__Auth0UserI__6AE5BEB7");

			entity.HasOne(d => d.CurrencyCodeNavigation)
				.WithMany(p => p.Debts)
				.HasPrincipalKey(p => p.AlphaCode)
				.HasForeignKey(d => d.CurrencyCode)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK__Debt__CurrencyCo__6CCE0729");
		});

		modelBuilder.Entity<ExchangeRate>(entity =>
		{
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
				.HasConstraintName("FK__ExchangeR__BaseC__530E3526");

			entity.HasOne(d => d.QuoteCurrencyNavigation)
				.WithMany(p => p.ExchangeRateQuoteCurrencyNavigations)
				.HasPrincipalKey(p => p.AlphaCode)
				.HasForeignKey(d => d.QuoteCurrency)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK__ExchangeR__Quote__54F67D98");
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
				.HasConstraintName("FK__SessionLo__Auth0__67152DD3");
		});

		modelBuilder.Entity<Snowflake>(entity =>
		{
			entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");

			entity.Property(e => e.Auth0UserId).HasMaxLength(75);

			entity.Property(e => e.CurrencyCode)
				.HasMaxLength(3)
				.HasDefaultValueSql("('USD')");

			entity.Property(e => e.EndingAt).HasDefaultValueSql("(dateadd(year,(45),getdate()))");

			entity.Property(e => e.NickName).HasMaxLength(50);

			entity.Property(e => e.StartingAt).HasDefaultValueSql("(getdate())");

			entity.HasOne(d => d.Auth0User)
				.WithMany(p => p.Snowflakes)
				.HasPrincipalKey(p => p.Auth0UserId)
				.HasForeignKey(d => d.Auth0UserId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK__Snowflake__Auth0__709E980D");

			entity.HasOne(d => d.CurrencyCodeNavigation)
				.WithMany(p => p.Snowflakes)
				.HasPrincipalKey(p => p.AlphaCode)
				.HasForeignKey(d => d.CurrencyCode)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK__Snowflake__Curre__746F28F1");
		});

		modelBuilder.Entity<StrategyType>(entity =>
		{
			entity.ToTable("StrategyType");

			entity.Property(e => e.Type).HasMaxLength(50);
		});

		modelBuilder.Entity<UserProfile>(entity =>
		{
			entity.ToTable("UserProfile");

			entity.HasIndex(e => e.Auth0UserId, "UQ__UserProf__1C8F4290BFE56211")
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

			entity.Property(e => e.SelectedStrategy).HasDefaultValueSql("((1))");

			entity.Property(e => e.TotalAmountOwed)
				.HasColumnType("decimal(18, 2)")
				.HasDefaultValueSql("((0))");

			entity.Property(e => e.UserTypeId).HasDefaultValueSql("((1))");

			entity.HasOne(d => d.SelectedStrategyNavigation)
				.WithMany(p => p.UserProfiles)
				.HasForeignKey(d => d.SelectedStrategy)
				.HasConstraintName("FK__UserProfi__Selec__60683044");
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