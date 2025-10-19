// UnhandledCircuitExceptionHandler.cs

using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

public class UnhandledCircuitExceptionHandler : CircuitHandler
{
    // Loglayıcıyı DI üzerinden alıyoruz.
    private readonly ILogger<UnhandledCircuitExceptionHandler> _logger;

    public UnhandledCircuitExceptionHandler(ILogger<UnhandledCircuitExceptionHandler> logger)
    {
        _logger = logger;
    }

    // Yalnızca Devrenin Kapanmasını Dinle
    public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        // Not: Detaylı Stack Trace bilgisi, CircuitHost loglama kategorisi tarafından
        // otomatik olarak loglanacaktır (Adım 2 ile garanti ediyoruz).
        _logger.LogWarning("Blazor Devresi kapatıldı. Detaylı hata için sunucu loglarını kontrol edin.");

        return Task.CompletedTask;
    }
}