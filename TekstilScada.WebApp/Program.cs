// Dosya: TekstilScada.WebApp/Program.cs

using Microsoft.AspNetCore.Components.Server.Circuits;
using TekstilScada.WebApp.Components;
using TekstilScada.WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// --- DOÐRU YAPILANDIRMA ---

// 1. "WebApiClient" adýyla özel bir HttpClient yapýlandýrýyoruz.
builder.Services.AddHttpClient("WebApiClient", client =>
{
    // LÜTFEN WebAPI projenizin çalýþtýðý PORT numarasýný burada kontrol edin!
    // Genellikle 7000'li bir sayýdýr.
    client.BaseAddress = new Uri("http://192.168.1.104:7039");
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
    var httpClient = httpClientFactory.CreateClient("WebApiClient"); // Ýsmine göre doðru istemciyi istiyoruz.
    return new ScadaDataService(httpClient);
});

// --- YAPILANDIRMA SONU ---
builder.Services.AddScoped<CircuitHandler, UnhandledCircuitExceptionHandler>();
builder.Services.AddLogging(); // Logger kullanmak için gerekli (zaten olabilir)

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
        // Yalnýzca /_blazor (Blazor Circuit) yolunda oluþan hatalarý yakalamaya odaklan
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