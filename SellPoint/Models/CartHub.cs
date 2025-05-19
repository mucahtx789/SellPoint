using Microsoft.AspNetCore.SignalR;

namespace SellPoint.Models
{
    public class CartHub : Hub
    {
        public async Task NotifyCartChanged(int customerId)
        {
            await Clients.All.SendAsync("CartUpdated", customerId);
        }
    }
}
