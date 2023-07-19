using BLL.Configurations;
using DAL.Interfaces;
using DAL.Models;
using DAL.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;

try
{
	var builder = WebApplication.CreateBuilder(args);

	// Configure Serilog
	Log.Logger = new LoggerConfiguration()
		.ReadFrom.Configuration(builder.Configuration)
		.CreateLogger();

	Log.Information("App server is starting up");

	// Add services to the container.
	builder.Services.AddControllersWithViews();
	builder.Services.AddRazorPages();

	//Configure AddAutoMapper
	builder.Services.AddAutoMapper(typeof(AutoMapperConfiguration));

	// Add services for the DB.
	builder.Services.AddDbContext<DebtSnowballerContext>();
	builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();

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
	builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

	builder.Services.AddCors(options =>
	{
		options.AddPolicy("Open", builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
	});

	builder.Services.AddControllers();

	// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
	builder.Services.AddEndpointsApiExplorer();
	builder.Services.AddSwaggerGen();

	builder.Host.UseSerilog(); // Use Serilog as the logging framework

	var app = builder.Build();

	// Configure the HTTP request pipeline.
	if (app.Environment.IsDevelopment())
	{
		app.UseWebAssemblyDebugging();
	}
	else
	{
		app.UseExceptionHandler("/Error");
		// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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

	app.Run();
}
catch (Exception ex)
{
	Log.Fatal(ex, "Application start-up failed");
}
finally
{
	Log.CloseAndFlush();
}