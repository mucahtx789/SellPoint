using Microsoft.AspNetCore.SignalR;

namespace SellPoint.Hubs
{
    public class ProductHub : Hub
    {
        public async Task NotifyProductChanged()
        {
            await Clients.All.SendAsync("ProductUpdated");
        }
    }
}
