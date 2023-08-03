using Blazor.Extensions.Logging;
using DebtSnowballer.Client;
using DebtSnowballer.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
	.AddHttpClient<IDebtService, DebtService>(client =>
		client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
	.AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services
	.AddHttpClient<IUserProfileService, UserProfileService>(client =>
		client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
	.AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services
	.AddHttpClient<ISnowflakeService, SnowflakeService>(client =>
		client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
	.AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services.AddOidcAuthentication(options =>
{
	builder.Configuration.Bind("Auth0", options.ProviderOptions);
	options.ProviderOptions.ResponseType = "code";
	options.ProviderOptions.AdditionalProviderParameters.Add("audience", builder.Configuration["Auth0:Audience"]);
});

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

// Configure logging
builder.Logging.SetMinimumLevel(LogLevel.Information);
builder.Logging.AddBrowserConsole();

WebAssemblyHost host = builder.Build();

// Log a message when the application starts up
ILogger<Program> logger = host.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("App client is starting up");

await host.RunAsync();