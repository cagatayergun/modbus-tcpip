// Dosya: TekstilScada.WebAPI/Services/PlcPollingBackgroundService.cs

using TekstilScada.Repositories;
using TekstilScada.Services;

public class PlcPollingBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PlcPollingBackgroundService> _logger;

    public PlcPollingBackgroundService(IServiceProvider serviceProvider, ILogger<PlcPollingBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("PLC Polling Service is starting.");

        try
        {
            // Kapsam (scope) oluşturarak servisleri alıyoruz.
            using (var scope = _serviceProvider.CreateScope())
            {
                _logger.LogInformation("Resolving services from scope...");

                var pollingService = scope.ServiceProvider.GetRequiredService<PlcPollingService>();
                var machineRepository = scope.ServiceProvider.GetRequiredService<MachineRepository>();

                _logger.LogInformation("Services resolved successfully.");
                _logger.LogInformation("Getting enabled machines from database...");

                var machines = machineRepository.GetAllEnabledMachines();

                _logger.LogInformation($"{machines.Count} enabled machines found. Starting polling service...");

                pollingService.Start(machines);

                _logger.LogInformation("PLC Polling Service has been started successfully.");
            }
        }
        catch (Exception ex)
        {
            // Eğer başlangıç sırasında herhangi bir hata olursa, bunu konsola detaylıca yaz.
            _logger.LogError(ex, "FATAL ERROR: PlcPollingBackgroundService could not start.");
        }

        // Servisin uygulama kapanana kadar çalışmasını sağlar.
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    // StopAsync metodu aynı kalabilir...
    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("PLC Polling Service is stopping.");
        // ... (içeriği aynı)
        await base.StopAsync(stoppingToken);
    }
}