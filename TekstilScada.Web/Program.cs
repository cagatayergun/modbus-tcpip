using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TekstilScada.Web;
using Microsoft.AspNetCore.Components.Authorization;
using TekstilScada.Web.Auth;
using Blazored.LocalStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// JWT Handler'ý kaydet
builder.Services.AddScoped<JwtHttpClientHandler>();

// HttpClient servisini kaydet ve JWT Handler'ý kullanmasýný söyle
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7253/") });

builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

await builder.Build().RunAsync();