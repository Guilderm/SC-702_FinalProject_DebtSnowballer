using Blazor.Extensions.Logging;
using DebtSnowballer.Client;
using DebtSnowballer.Client.ClientSideServices;
using DebtSnowballer.Client.ClientSideServices.SolvencyEngine;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
	.AddHttpClient<IDebtService, DebtService>(client =>
		client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
	.AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services
	.AddHttpClient<IUserService, UserService>(client =>
		client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
	.AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services
	.AddHttpClient<ISnowflakeService, SnowflakeService>(client =>
		client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
	.AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services
	.AddHttpClient<IMultiPurposeService, MultiPurposeService>(client =>
		client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
	.AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();


builder.Services.AddTransient<LoanAmortizationCreator>();
builder.Services.AddTransient<SolvencyPlanCreator>();
builder.Services.AddTransient<IPaymentInstallmentFactory, PaymentInstallmentFactory>();
builder.Services.AddTransient<SnowflakesScheduleCreator>();
builder.Services.AddScoped<ISolvencyPlanner, SolvencyPlanner>();


builder.Services.AddOidcAuthentication(options =>
{
	builder.Configuration.Bind("Auth0", options.ProviderOptions);
	options.ProviderOptions.ResponseType = "code";
	options.ProviderOptions.AdditionalProviderParameters.Add("audience", builder.Configuration["Auth0:Audience"]);
});

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.AddMudServices();

// Configure logging
builder.Logging.SetMinimumLevel(LogLevel.Information);
builder.Logging.AddBrowserConsole();

WebAssemblyHost host = builder.Build();

// Log a message when the application starts up
ILogger<Program> logger = host.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("App client is starting up");

await host.RunAsync();