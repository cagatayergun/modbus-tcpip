using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace TekstilScada.Api.Hubs
{
    [Authorize]
    public class MachineHub : Hub
    {
        // İstemcilerin sunucuya çağırabileceği metotlar buraya eklenebilir.
        // Örneğin, bir web istemcisi belirli bir makinenin detaylarını istemek için
        // bu metodu çağırabilir.
        public Task JoinMachineGroup(int machineId)
        {
            // İstemciyi belirli bir makine ID'sine ait gruba ekle.
            // Bu sayede sadece ilgili makinenin verisi o istemciye gönderilir.
            return Groups.AddToGroupAsync(Context.ConnectionId, $"machine-{machineId}");
        }

        // Bir istemci gruptan ayrılırken çağrılır.
        public Task LeaveMachineGroup(int machineId)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, $"machine-{machineId}");
        }
    }
}