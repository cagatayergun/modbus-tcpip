// Dosya: TekstilScada.WebApp/Program.cs (TEMÝZLENMÝÞ VE DÜZELTÝLMÝÞ)

using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.Extensions.Logging;
// using Microsoft.AspNetCore.Authentication.Cookies; // --- GEREKLÝ DEÐÝL
// using Microsoft.AspNetCore.Authentication; // --- GEREKLÝ DEÐÝL
using System.Text.Json;
using TekstilScada.WebApp.Components;
using TekstilScada.WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

// --- 1. BU BLOKU GERÝ EKLEYÝN (YORUMLARI KALDIRIN) ---
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Login yolu doðru belirtilmiþti, kalsýn
        options.LoginPath = "/login";
    });

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddCircuitOptions(options => { options.DetailedErrors = true; });

// 1. Blazored Local Storage'ý ekle
builder.Services.AddBlazoredLocalStorage();

// --- 2. ADDSERVICES.ADDAUTHORIZATION() YERÝNE DAHA TEMEL OLANI KULLAN ---
// Bu, Blazor'un [Authorize] özniteliklerini tanýmasý için yeterlidir.
builder.Services.AddAuthorizationCore();

// 3. CustomAuthStateProvider kaydý (Bu doðru)
builder.Services.AddScoped<CustomAuthStateProvider>(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient("WebApiClient");
    var localStorage = sp.GetRequiredService<ILocalStorageService>();
    return new CustomAuthStateProvider(httpClient, localStorage);
});

// 4. CustomAuthStateProvider'ý ana kimlik doðrulama saðlayýcýsý olarak ata (Bu doðru)
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<CustomAuthStateProvider>());


// HttpClient yapýlandýrmasý (Bu doðru)
builder.Services.AddHttpClient("WebApiClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:7039");
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    return new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
    };
});

// ScadaDataService kaydý (Bu doðru)
builder.Services.AddSingleton(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient("WebApiClient");
    return new ScadaDataService(httpClient);
});

// --- Diðer servisler (Bunlar doðru) ---
builder.Services.AddScoped<CircuitHandler, UnhandledCircuitExceptionHandler>();
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();



// ... (Kalan kodun ayný kalabilir, ScadaDataService baþlatma vb.) ...

var scadaDataService = app.Services.GetRequiredService<ScadaDataService>();
await scadaDataService.InitializeAsync();
var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
var circuitLogger = loggerFactory.CreateLogger("CircuitLogger");
app.Use(async (context, next) =>
{
    try
    {
        await next(context);
    }
    catch (Exception ex)
    {
        if (context.Request.Path.StartsWithSegments("/_blazor"))
        {
            circuitLogger.LogError(ex, ">>> KRÝTÝK BLZOR DEVRE HATASI YAKALANDI! <<<");
            context.Response.ContentType = "text/plain";
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Blazor devre hatasý: Sunucu baðlantýsý kesildi. Detaylar için sunucu loglarýna bakýn.");
            return;
        }
        throw;
    }
});
app.Run();