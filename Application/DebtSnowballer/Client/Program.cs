using Blazor.Extensions.Logging;
using DebtSnowballer.Client;
using DebtSnowballer.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddOidcAuthentication(options =>
{
	builder.Configuration.Bind("Auth0", options.ProviderOptions);
	options.ProviderOptions.ResponseType = "code";
	options.ProviderOptions.AdditionalProviderParameters.Add("audience", builder.Configuration["Auth0:Audience"]);
});

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDebtService, DebtService>();

// Configure logging
builder.Logging.SetMinimumLevel(LogLevel.Information);
builder.Logging.AddBrowserConsole();

var host = builder.Build();

// Log a message when the application starts up
var logger = host.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("App client is starting up");

await host.RunAsync();