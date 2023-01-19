using Microsoft.AspNetCore.SignalR;
namespace SignalRChat.Hubs;

public class CoinHub : Hub
{
    public async Task SendMessage(string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", message);
    }
}