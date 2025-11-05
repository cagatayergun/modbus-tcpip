// Dosya: TekstilScada.WebApp/Program.cs (TEMÝZLENMÝÞ VE DÜZELTÝLMÝÞ)

using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TekstilScada.WebApp.Components;
using TekstilScada.WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

// --- 1. ÇEKÝRDEK KÝMLÝK DOÐRULAMA SERVÝSLERÝ EKLENDÝ ---
// Servislerin kayýtlý olmasý, "IAuthenticationService" ve "No scheme"
// hatalarýný engeller.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";

        // --- YÖNLENDÝRME DÖNGÜSÜ ÝÇÝN DÜZELTME ---
        options.Events.OnRedirectToLogin = context =>
        {
            // KRÝTÝK KONTROL: Eðer istek ZATEN /login sayfasýna
            // gidiyorsa, ASLA tekrar yönlendirme yapma.
            // Bu, "boþ sayfa" ve "manuel giriþ" sorunlarýný
            // (sonsuz döngüyü) çözecektir.
            if (context.Request.Path == options.LoginPath)
            {
                // Sadece olayýn tamamlanmasýna izin ver,
                // sayfanýn (Login.razor) yüklenmesini engelleme.
                return Task.CompletedTask;
            }

            // Eðer istek /login dýþýnda bir sayfaya (örn: '/')
            // yapýlýyorsa, normal þekilde /login'e yönlendir.
            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        };
        // --- DÜZELTMENÝN SONU ---
    });


// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddCircuitOptions(options => { options.DetailedErrors = true; });

// 1. Blazored Local Storage'ý ekle (Bu DOÐRU)
builder.Services.AddBlazoredLocalStorage();

// 2. AuthorizationCore (Bu DOÐRU, Blazor'un [Authorize] özniteliðini tanýmasý için bu kalmalý)
builder.Services.AddAuthorizationCore();

// 3. CustomAuthStateProvider kaydý (Bu DOÐRU)
builder.Services.AddScoped<CustomAuthStateProvider>(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient("WebApiClient");
    var localStorage = sp.GetRequiredService<ILocalStorageService>();

    // YENÝ: Logger'ý da servisten alýp provider'a iletiyoruz
    var logger = sp.GetRequiredService<ILogger<CustomAuthStateProvider>>();

    return new CustomAuthStateProvider(httpClient, localStorage, logger); // <-- logger'ý buraya ekleyin
});

// 4. CustomAuthStateProvider'ý ana kimlik doðrulama saðlayýcýsý olarak ata (Bu DOÐRU)
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<CustomAuthStateProvider>());


// HttpClient yapýlandýrmasý (Bu doðru)
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5000";

// HttpClient yapýlandýrmasý (Bu doðru)
builder.Services.AddHttpClient("WebApiClient", client =>
{
    // client.BaseAddress = new Uri("http://localhost:7039"); // ESKÝ SATIR
    client.BaseAddress = new Uri(apiBaseUrl); // YENÝ SATIR
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
    // app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// --- BU ÝKÝ SATIR YORUMDA KALMALI ---
// Bu middleware'ler, F5'te /login yönlendirmesine neden olan
// çerez (Cookie) tabanlý sistemi tetikler.
// app.UseAuthentication();
// app.UseAuthorization();

// --- SON ÇÖZÜM: ÖN YÜKLEMEYÝ (PRERENDERING) KAPAT ---
// Prerendering ayarý App.razor dosyasýnda zaten (prerender: false) olarak yapýldý.
// Buradaki satýrýn parametresiz olmasý gerekiyor.
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode(); // <-- HATA DÜZELTÝLDÝ: Parametre kaldýrýldý



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

