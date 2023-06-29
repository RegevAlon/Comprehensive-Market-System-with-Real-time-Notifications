using Microsoft.AspNetCore.SignalR;


namespace ServerMarket.API
{
    public class NotificationHub : Hub
    {
        public string connectionId;

        public override async Task OnConnectedAsync()
        {
            var sessionId = Context.ConnectionId;
            connectionId = sessionId;


            await base.OnConnectedAsync();
            await Clients.Caller.SendAsync("ReceiveSessionId", sessionId);

        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendNotification(string sessionId, string message)
        {
            await Clients.Client(sessionId).SendAsync("ReceiveNotification", message);
        }
    }
}