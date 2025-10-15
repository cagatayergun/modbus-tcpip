// Dosya: TekstilScada.WebAPI/Program.cs

using TekstilScada.Repositories;
using TekstilScada.Services;
using TekstilScada.WebAPI.Hubs;
using TekstilScada.WebAPI.Services;

var builder = WebApplication.CreateBuilder(args);
// 1. Configuration'dan (appsettings.json) veritabaný baðlantý dizesini oku.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Okunan baðlantý dizesini Core katmanýndaki statik AppConfig sýnýfýna ata.
// Bu sayede projedeki tüm Repository sýnýflarý doðru baðlantý dizesini kullanabilir.
TekstilScada.Core.AppConfig.SetConnectionString(connectionString);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

// === TekstilScada Servislerini Buraya Ekliyoruz ===
// Proje boyunca tek bir örneði olacak tüm servisleri Singleton olarak kaydediyoruz.
// Bu, arka plan servislerinin ve anlýk veri akýþýnýn tutarlý çalýþmasý için gereklidir.
builder.Services.AddSingleton<MachineRepository>();
builder.Services.AddSingleton<AlarmRepository>();
builder.Services.AddSingleton<ProductionRepository>();
builder.Services.AddSingleton<ProcessLogRepository>();
builder.Services.AddSingleton<RecipeRepository>();
builder.Services.AddSingleton<UserRepository>();
builder.Services.AddSingleton<DashboardRepository>();
builder.Services.AddSingleton<PlcPollingService>();
builder.Services.AddSingleton<SignalRBridgeService>();

// PLC Polling servisini arka planda çalýþacak bir hizmet olarak ekliyoruz.
builder.Services.AddHostedService<PlcPollingBackgroundService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});
// === Servis Ekleme Sonu ===

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.MapHub<ScadaHub>("/scadaHub");

// Köprü servisinin uygulama baþlarken aktif olmasýný saðlýyoruz.
app.Services.GetRequiredService<SignalRBridgeService>();

app.Run();