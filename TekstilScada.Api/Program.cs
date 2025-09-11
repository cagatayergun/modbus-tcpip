// Gerekli using ifadeleri
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TekstilScada.Api.Hubs;
using TekstilScada.Api.Services;
using TekstilScada.Core;
using TekstilScada.Repositories; // Bu using önemli
using TekstilScada.Services;     // Bu using önemli

var builder = WebApplication.CreateBuilder(args);

//--------------------------------------------------------------------
// 1. ADIM: TÜM SERVÝSLERÝ BURADA TANIMLA ('builder.Services')
//--------------------------------------------------------------------

// Baðlantý dizesini ve AppConfig'i ayarla
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
AppConfig.SetConnectionString(connectionString);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JWT Kimlik Doðrulama servisini DOÐRU YERE taþýdýk
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        // Konfigürasyonu en saðlam yöntemle oku
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:Secret").Value)),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});

// Yetkilendirme (Authorization) servisini DOÐRU YERE taþýdýk
builder.Services.AddAuthorization();

// CORS servisi
builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorAppPolicy", policyBuilder =>
    {
        policyBuilder.AllowAnyOrigin()
                     .AllowAnyMethod()
                     .AllowAnyHeader();
    });
});

// SignalR ve diðer tüm servisler
builder.Services.AddSignalR();
builder.Services.AddSingleton<AlarmRepository>();
builder.Services.AddSingleton<MachineRepository>();
builder.Services.AddSingleton<ProductionRepository>();
builder.Services.AddSingleton<RecipeRepository>();
builder.Services.AddSingleton<ProcessLogRepository>();
builder.Services.AddSingleton<CostRepository>();
builder.Services.AddSingleton<UserRepository>();
builder.Services.AddSingleton<DashboardRepository>();
builder.Services.AddSingleton<RecipeConfigurationRepository>();
builder.Services.AddSingleton<PlcOperatorRepository>();
builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<PlcPollingService>();
builder.Services.AddSingleton<IHostedService, PlcPollingHostedService>();
builder.Services.AddSingleton<IHostedService, SignalRNotifierService>();
builder.Services.AddSingleton<FtpTransferService>();

//--------------------------------------------------------------------
// 2. ADIM: UYGULAMAYI OLUÞTUR ('builder.Build()')
// Bu satýrdan sonra 'builder.Services' ile servis eklenemez.
//--------------------------------------------------------------------
var app = builder.Build();

//--------------------------------------------------------------------
// 3. ADIM: HTTP ISTEK HATTINI YAPILANDIR ('app.Use...')
//--------------------------------------------------------------------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("BlazorAppPolicy");

// Kimlik doðrulama ve Yetkilendirme middleware'lerini doðru sýrayla ekle
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<MachineHub>("/machine-hub");

app.Run();