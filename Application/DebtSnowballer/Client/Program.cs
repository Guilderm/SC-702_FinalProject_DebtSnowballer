using DebtSnowballer.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddOidcAuthentication(options =>
{
	builder.Configuration.Bind("Auth0", options.ProviderOptions);
	options.ProviderOptions.ResponseType = "code";
    // options.ProviderOptions.AdditionalProviderParameters.Add("audience", builder.Configuration["Auth0:Audience"]);
    options.ProviderOptions.Authority = "https://dev-vsyv0novxxjhled7.us.auth0.com";
    options.ProviderOptions.ClientId = "XY2rNTvBnjQw8brvGJVzhosp6MBAHHxd";
});

await builder.Build().RunAsync();