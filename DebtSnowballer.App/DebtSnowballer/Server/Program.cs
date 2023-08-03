using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Server.BLL.Configurations;
using Server.BLL.Services;
using Server.DAL.Interfaces;
using Server.DAL.Models;
using Server.DAL.Repositories;

try
{
	WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

	// Configure Serilog
	Log.Logger = new LoggerConfiguration()
		.ReadFrom.Configuration(builder.Configuration)
		.CreateLogger();

	Log.Information("App server is starting up");

	// Add services to the container.
	Log.Information("Configuring services...");
	builder.Services.AddControllersWithViews();
	builder.Services.AddRazorPages();

	//Configure AddAutoMapper
	Log.Information("Adding AutoMapper...");
	builder.Services.AddAutoMapper(typeof(AutoMapperConfiguration));

	// Add services for the DB.
	Log.Information("Configuring DbContext...");
	builder.Services.AddDbContext<DebtSnowballerContext>(options =>
		options.UseSqlServer(builder.Configuration.GetConnectionString("AzureBDConnection")));

	Log.Information("Adding scoped services...");
	builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
	builder.Services.AddScoped<DebtManagement>();
	builder.Services.AddScoped<UserProfileManagement>();
	builder.Services.AddScoped<CurrencyService>();

	Log.Information("Configuring authentication...");
	builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
		.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, c =>
		{
			c.Authority = $"https://{builder.Configuration["Auth0:Domain"]}";
			c.TokenValidationParameters = new TokenValidationParameters
			{
				ValidAudience = builder.Configuration["Auth0:Audience"],
				ValidIssuer = $"https://{builder.Configuration["Auth0:Domain"]}"
			};
		});

	//This was added at the start of using data from an API module 5
	Log.Information("Adding HttpContextAccessor...");
	builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

	Log.Information("Configuring CORS...");
	builder.Services.AddCors(options =>
	{
		options.AddPolicy("Open", builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
	});

	Log.Information("Adding controllers...");
	builder.Services.AddControllers();

	// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
	Log.Information("Configuring Swagger...");
	builder.Services.AddEndpointsApiExplorer();
	builder.Services.AddSwaggerGen();

	Log.Information("Using Serilog...");
	builder.Host.UseSerilog();

	Log.Information("Building application...");
	WebApplication app = builder.Build();

	// Configure the HTTP request pipeline.
	Log.Information("Configuring HTTP request pipeline...");
	if (app.Environment.IsDevelopment())
	{
		app.UseWebAssemblyDebugging();
	}
	else
	{
		app.UseExceptionHandler("/Error");
		app.UseHsts();
	}

	app.UseSwagger();
	app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); });

	app.UseCors("Open");

	app.UseHttpsRedirection();

	app.UseBlazorFrameworkFiles();
	app.UseStaticFiles();

	app.UseRouting();

	app.UseAuthentication();
	app.UseAuthorization();

	app.MapRazorPages();
	app.MapControllers();
	app.MapFallbackToFile("index.html");

	Log.Information("Running application...");
	app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
	Log.Fatal(ex, "Host terminated unexpectedly.");
}
finally
{
	Log.CloseAndFlush();
}