using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TekstilScada.Core;
using TekstilScada.Repositories;
using TekstilScada.Services;
using Microsoft.AspNetCore.Authorization;
using TekstilScada.Api.Hubs;
using TekstilScada.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Baðlantý dizesini appsettings.json'dan al
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
AppConfig.SetConnectionString(connectionString);

// Add services to the container.
builder.Services.AddControllers();

// Repositories ve Servisleri DI konteynerine Singleton olarak ekle
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

// PLC Polling Service ve onun sarmalayýcýsýný Singleton olarak kaydet.
builder.Services.AddSingleton<PlcPollingService>();
builder.Services.AddSingleton<IHostedService, PlcPollingHostedService>();

// SignalR Notifier Service'i Hosted Service olarak kaydet
builder.Services.AddSingleton<IHostedService, SignalRNotifierService>();
builder.Services.AddSingleton<FtpTransferService>();

// SignalR servislerini ekle
builder.Services.AddSignalR();

// CORS servisini ekle ve Blazor uygulamanýn origin'ine izin ver
builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorAppPolicy",
        builder =>
        {
            builder.WithOrigins("https://localhost:7264") // Blazor uygulamanýn adresi
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});

// JWT Kimlik Doðrulamasýný Ekle
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Secret"])),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});

// Yetkilendirme (Authorization) hizmetini ekle
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// CORS politikasýný HTTP isteði hattýna ekle
app.UseCors("BlazorAppPolicy");

// Kimlik Doðrulama ve Yetkilendirme middleware'lerini ekle
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// SignalR Hub'ýný bir uç noktaya eþle
app.MapHub<MachineHub>("/machine-hub");

app.Run();