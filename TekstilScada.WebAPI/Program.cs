// Dosya: TekstilScada.WebAPI/Program.cs

using TekstilScada.Repositories;
using TekstilScada.Services;
using TekstilScada.WebAPI.Hubs;
using TekstilScada.WebAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer; // Eklendi
using Microsoft.IdentityModel.Tokens; // Eklendi
using System.Text; // Eklendi
using Microsoft.AspNetCore.Authorization; // Eklendi

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

// === JWT Yapýlandýrmasý BAÞLANGIÇ ===
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is missing");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization(); // Yetkilendirme servisini ekle
// === JWT Yapýlandýrmasý SONU ===


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
builder.Services.AddSingleton<FtpTransferService>();
builder.Services.AddSingleton<RecipeConfigurationRepository>();
// PLC Polling servisini arka planda çalýþacak bir hizmet olarak ekliyoruz.
builder.Services.AddHostedService<PlcPollingBackgroundService>();
//builder.Services.AddScoped<FtpService>();
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
// KRÝTÝK SIRALAMA: Kimlik doðrulama, Yetkilendirmeden önce gelmelidir.
app.UseAuthentication(); // JWT doðrulamasýný etkinleþtir
app.UseAuthorization(); // Yetkilendirme kurallarýný etkinleþtir
app.MapControllers();
app.MapHub<ScadaHub>("/scadaHub");

// Köprü servisinin uygulama baþlarken aktif olmasýný saðlýyoruz.
app.Services.GetRequiredService<SignalRBridgeService>();

app.Run();
