// Dosya: TekstilScada.WebApp/Program.cs

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
    client.BaseAddress = new Uri("https://localhost:7039");
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

app.Run();