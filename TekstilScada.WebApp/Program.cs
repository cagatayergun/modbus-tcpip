// Dosya: TekstilScada.WebApp/Program.cs (HATA CS1061 DÜZELTÝLDÝ)

using Microsoft.AspNetCore.Components.Server.Circuits;
using TekstilScada.WebApp.Components;
using TekstilScada.WebApp.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// KRÝTÝK DÜZELTME: UseAuthentication'ý desteklemek için boþ bir þema ekliyoruz.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Sisteme Blazor giriþ sayfanýzýn nerede olduðunu söyleyin
        options.LoginPath = "/login";
    });

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    // HATA ÇÖZÜMÜ: .AddServerComponents() ve .AddServerSideBlazor() çaðrýlarý kaldýrýldý.
    .AddCircuitOptions(options => { options.DetailedErrors = true; }); // Detaylý hatalar için


// 1. Blazored Local Storage'ý ekle
builder.Services.AddBlazoredLocalStorage();

// 2. Blazor Yetkilendirme (AuthorizationCore yerine, full yetkilendirme servislerini kullan)
builder.Services.AddAuthorization();

// 3. CustomAuthStateProvider kaydý
builder.Services.AddScoped<CustomAuthStateProvider>(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient("WebApiClient");
    var localStorage = sp.GetRequiredService<ILocalStorageService>();
    return new CustomAuthStateProvider(httpClient, localStorage);
});

// 4. CustomAuthStateProvider'ý ana kimlik doðrulama saðlayýcýsý olarak ata
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<CustomAuthStateProvider>());


// 1. "WebApiClient" adýyla özel bir HttpClient yapýlandýrýyoruz.
builder.Services.AddHttpClient("WebApiClient", client =>
{
    client.BaseAddress = new Uri("http://192.168.1.101:7039");
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    // Geliþtirme ortamýnda SSL sertifika hatalarýný görmezden gel
    return new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
    };
});

// 2. ScadaDataService'i, yukarýda yapýlandýrdýðýmýz özel HttpClient'ý alacak þekilde kaydediyoruz.
builder.Services.AddSingleton(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient("WebApiClient");
    return new ScadaDataService(httpClient);
});

// --- YAPILANDIRMA SONU ---
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

// KRÝTÝK: UseAuthentication ve UseAuthorization, MapRazorComponents'tan ÖNCE OLMALIDIR.
app.UseAuthentication();
app.UseAuthorization();

// KRÝTÝK: Yetkilendirme artýk FallbackPolicy ve Routes.razor tarafýndan yönetilecek
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


// Uygulama baþlarken ScadaDataService'i baþlatýyoruz.
var scadaDataService = app.Services.GetRequiredService<ScadaDataService>();
await scadaDataService.InitializeAsync();
// *** KRÝTÝK ADIM: Blazor Server Devre Hata Ýþleyicisini Ekliyoruz ***
var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();

// CircuitHost için Logger tanýmlýyoruz
var circuitLogger = loggerFactory.CreateLogger("CircuitLogger");

// Uygulamanýn en sonunda, tüm Blazor hatalarýný yakala
app.Use(async (context, next) =>
{
    try
    {
        await next(context);
    }
    catch (Exception ex)
    {
        // Yalnýzca /_blazor (Blazor Circuit) yolunda oluþan hatalara odaklan
        if (context.Request.Path.StartsWithSegments("/_blazor"))
        {
            // Detaylý hatayý sunucu konsoluna yazdýr
            circuitLogger.LogError(ex, ">>> KRÝTÝK BLZOR DEVRE HATASI YAKALANDI! <<<");

            // Kullanýcýya genel bir hata mesajý gönder, böylece uygulama donmaz
            context.Response.ContentType = "text/plain";
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Blazor devre hatasý: Sunucu baðlantýsý kesildi. Detaylar için sunucu loglarýna bakýn.");
            return;
        }
        throw; // Diðer HTTP hatalarýný normal þekilde fýrlat
    }
});
app.Run();
