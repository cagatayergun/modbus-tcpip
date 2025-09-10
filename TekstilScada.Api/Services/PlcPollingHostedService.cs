using Microsoft.Extensions.Hosting;
using TekstilScada.Services;

public class PlcPollingHostedService : IHostedService
{
    private readonly PlcPollingService _pollingService;

    public PlcPollingHostedService(PlcPollingService pollingService)
    {
        _pollingService = pollingService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Windows uygulamasının kullandığı start metodunu çağır
        var machines = new TekstilScada.Repositories.MachineRepository().GetAllEnabledMachines();
        _pollingService.Start(machines);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // Windows uygulamasının kullandığı stop metodunu çağır
        _pollingService.Stop();
        return Task.CompletedTask;
    }
}