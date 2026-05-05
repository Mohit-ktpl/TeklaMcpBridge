using Contracts.Envelopes;
using Contracts.Interfaces;
using Contracts.Models;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Server.Hubs;
using Server.Services.Interface;
using static System.Runtime.InteropServices.JavaScript.JSType;
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

        public async Task<SharedResult> CreateSimpleBeamAsync(string userId)
        {
            string connectionId = _tracker.GetConnectionId(userId);

            if (string.IsNullOrEmpty(connectionId))
            {
                return new SharedResult { Success = false, Message = $"Local PC for user '{userId}' is not connected." };
            }
            Random rnd = new Random();
            string profile = "ISMB400";
            double x = rnd.Next(0, 10001), y = rnd.Next(0, 10001), h = rnd.Next(2000, 5000);
            var dto = new SimpleBeamCreateDto
            {
                Profile = profile,
                Material = "43A",
                StartX = x,
                StartY = y,
                StartH = 0,
                EndX = x,
                EndY = y,
                EndH = h,
            };
            var request = new GenericEnvelope
            {
                CommandName = "CreateSimpleBeam",
                Payload = JsonConvert.SerializeObject(dto)
            };

            // 4. SEND TO CLIENT: Invoke the client method and wait for response
            var response = await _hubContext.Clients.Client(connectionId).ExecuteGenericCommand(request);

            // 5. UNPACK RESULT
            return JsonConvert.DeserializeObject<SharedResult>(response.Payload);
        }

        public async Task<SharedResult> DeleteBeamAsync(string userId)
        {
            string connectionId = _tracker.GetConnectionId(userId);

            if (string.IsNullOrEmpty(connectionId))
            {
                return new SharedResult { Success = false, Message = $"Local PC for user '{userId}' is not connected." };
            }
            var request = new GenericEnvelope
            {
                CommandName = "DeleteRandomBeam",
                Payload = "{}" // Send an empty JSON object instead of null
            };

            var response = await _hubContext.Clients.Client(connectionId).ExecuteGenericCommand(request);
            return JsonConvert.DeserializeObject<SharedResult>(response.Payload);


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

        public async Task<SharedResult> UpdateBeamClassByProfileAsync(string userId, string profileName, int newClass)
        {

            string connectionId = _tracker.GetConnectionId(userId);

            if (string.IsNullOrEmpty(connectionId))
            {
                return new SharedResult { Success = false, Message = $"Local PC for user '{userId}' is not connected." };
            }
            ;

            // 1. FETCH STATE FROM LOCAL APP
            var getRequest = new GenericEnvelope { CommandName = "GetAllBeamsSummary", Payload = "{}" };
            var getResponse = await _hubContext.Clients.Client(connectionId).ExecuteGenericCommand(getRequest);

            var allBeams = JsonConvert.DeserializeObject<List<BeamSummaryDto>>(getResponse.Payload);

            // 2. SERVER-SIDE BUSINESS LOGIC (The Brain)
            var guidsToUpdate = allBeams
            .Where(b => b.Profile.Equals(profileName, StringComparison.OrdinalIgnoreCase))
            .Select(b => b.Guid)
            .ToList();
            if (!guidsToUpdate.Any())
            {
                return new SharedResult { Success = false, Message = $"No beams found with profile {profileName}." };
            }

            // 3. SEND COMMAND TO LOCAL APP
            var updateDto = new UpdateBeamsByGuidDto { TargetGuids = guidsToUpdate, NewClass = newClass };
            var updateRequest = new GenericEnvelope
            {
                CommandName = "UpdateBeamProfile",
                Payload = JsonConvert.SerializeObject(updateDto)
            };

            var updateResponse = await _hubContext.Clients.Client(connectionId).ExecuteGenericCommand(updateRequest);
            return JsonConvert.DeserializeObject<SharedResult>(updateResponse.Payload);


        }
    }
}
