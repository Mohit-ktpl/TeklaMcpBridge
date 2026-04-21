using Contracts.Envelopes;
using Contracts.Interfaces;
using Contracts.Models;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Server.Hubs;
using Server.Services.Interface;
namespace Server.Services.Implementation
{
    public class ModelingService : IModelingService
    {
        private readonly IHubContext<TeklaHub, ITeklaClient> _hubContext;
        private readonly IUserTracker _tracker;

        public ModelingService(IHubContext<TeklaHub, ITeklaClient> hubContext, IUserTracker tracker)
        {
            this._hubContext = hubContext;
            this._tracker = tracker;
        }
        public async Task<SharedResult> DesignAndCreateBeamAsync(string userId, double span, double load)
        {
            // 1. Resolve the specific connection ID
            string connectionId = _tracker.GetConnectionId(userId);

            if (string.IsNullOrEmpty(connectionId))
            {
                return new SharedResult { Success = false, Message = $"Local PC for user '{userId}' is not connected." };
            }
            // 2. HIDDEN LOGIC: Your secret engineering calculations
            string profile = (span * load > 50000) ? "ISMB400" : "ISMB300";

            var dto = new BeamCreateDto
            {
                Profile = profile,
                Material = "S355JR",
                StartX = 0,
                EndX = 1000,
            };

            // 3. PACK ENVELOPE
            var request = new GenericEnvelope
            {
                CommandName = "CreateBeam",
                Payload = JsonConvert.SerializeObject(dto)
            };

            // 4. SEND TO CLIENT: Invoke the client method and wait for response
            var response = await _hubContext.Clients.Client(connectionId).ExecuteGenericCommand(request);

            // 5. UNPACK RESULT
            return JsonConvert.DeserializeObject<SharedResult>(response.Payload);
        }
    }
}
