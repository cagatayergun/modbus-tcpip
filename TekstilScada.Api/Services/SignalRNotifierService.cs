using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using TekstilScada.Api.Hubs;
using TekstilScada.Models;
using TekstilScada.Services;
using System.Threading;
using System.Threading.Tasks;

namespace TekstilScada.Api.Services
{
    // IHostedService arayüzü sayesinde uygulama başladığında çalışacak
    public class SignalRNotifierService : IHostedService
    {
        private readonly PlcPollingService _pollingService;
        private readonly IHubContext<MachineHub> _hubContext;

        public SignalRNotifierService(PlcPollingService pollingService, IHubContext<MachineHub> hubContext)
        {
            _pollingService = pollingService;
            _hubContext = hubContext;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // PlcPollingService'in veri yenileme olayına abone ol
            _pollingService.OnMachineDataRefreshed += OnMachineDataRefreshed;
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Uygulama durduğunda aboneliği kaldır
            _pollingService.OnMachineDataRefreshed -= OnMachineDataRefreshed;
            return Task.CompletedTask;
        }

        private void OnMachineDataRefreshed(int machineId, FullMachineStatus status)
        {
            // Olay tetiklendiğinde, SignalR Hub'ı üzerinden istemcilere veri gönder
            // Sadece ilgili makineye abone olanlara gönder
            _hubContext.Clients.Group($"machine-{machineId}").SendAsync("ReceiveMachineStatus", status);
        }
    }
}