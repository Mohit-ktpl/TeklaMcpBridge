using Microsoft.AspNetCore.SignalR;
using Contracts.Interfaces;
using Server.Services.Interface;

namespace Server.Hubs
{
    public class TeklaHub : Hub<ITeklaClient>
    {
        private readonly IUserTracker _tracker;

        public TeklaHub(IUserTracker userTracker)
        {
            this._tracker = userTracker;
        }
        public override Task OnConnectedAsync()
        {
            // Read the custom userId we will pass from the local PC
            var httpContext = Context.GetHttpContext();
            var userId = httpContext?.Request.Query["userId"].ToString();

            if (!string.IsNullOrEmpty(userId))
            {
                _tracker.Add(userId, Context.ConnectionId);
                Console.WriteLine($"[Hub] User '{userId}' connected with ID: {Context.ConnectionId}");
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            _tracker.Remove(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }

    }
}
