// Dosya: TekstilScada.WebAPI/Services/SignalRBridgeService.cs

using Microsoft.AspNetCore.SignalR;
using TekstilScada.Models;
using TekstilScada.Services;
using TekstilScada.WebAPI.Hubs;

namespace TekstilScada.WebAPI.Services
{
    /// <summary>
    /// PlcPollingService'den gelen olayları dinler ve SignalR Hub'ına iletir.
    /// </summary>
    public class SignalRBridgeService
    {
        private readonly IHubContext<ScadaHub> _hubContext;
        private readonly PlcPollingService _pollingService;

        public SignalRBridgeService(IHubContext<ScadaHub> hubContext, PlcPollingService pollingService)
        {
            _hubContext = hubContext;
            _pollingService = pollingService;

            // Polling servisinden gelen veri yenileme olayına abone oluyoruz.
            _pollingService.OnMachineDataRefreshed += OnMachineDataRefreshedHandler;
        }

        // Olay tetiklendiğinde çalışacak metot.
        private void OnMachineDataRefreshedHandler(int machineId, FullMachineStatus status)
        {
            // Gelen veriyi tüm bağlı web tarayıcılarına "ReceiveMachineUpdate" mesajıyla gönder.
            _hubContext.Clients.All.SendAsync("ReceiveMachineUpdate", status);
        }
    }
}