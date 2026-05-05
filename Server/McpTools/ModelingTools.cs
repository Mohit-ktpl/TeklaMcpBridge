using ModelContextProtocol.Server;
using Server.Services.Implementation;
using Server.Services.Interface;
using System.ComponentModel;

namespace Server.McpTools
{
    [McpServerToolType]
    public class ModelingTools
    {
        private readonly IModelingService _modelingService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ModelingTools(IModelingService modelingService, IHttpContextAccessor httpContextAccessor)
        {
            _modelingService = modelingService;
            this._httpContextAccessor = httpContextAccessor;
        }
        [McpServerTool, Description("Designs and creates a beam in Tekla based on span and load")]
        public async Task<string> CreateBeam( double span, double load)
        {

            // 1. Grab the current web request
            var context = _httpContextAccessor.HttpContext;

            // 2. Extract the userId from the query string
            string? userId = context?.Request.Query["userId"].ToString();

            // 3. Fail gracefully if the ID is missing
            if (string.IsNullOrEmpty(userId))
            {
                return "Failed: Unauthorized. No userId was provided by the client.";
            }

            // 4. Pass the dynamic ID to your service
            var result = await _modelingService.DesignAndCreateBeamAsync(userId, span, load);

            return result.Success
                ? $"Success! Beam ID: {result.CreatedObjectGuid}"
                : $"Failed: {result.Message}";
        }

        [McpServerTool, Description("Creates a beam in Tekla")]
        public async Task<string> CreateSimpleBeam()
        {

            var context = _httpContextAccessor.HttpContext;
            string? userId = context?.Request.Query["userId"].ToString();

            if (string.IsNullOrEmpty(userId))
            {
                return "Failed: Unauthorized. No userId was provided by the client.";
            }

            var result = await _modelingService.CreateSimpleBeamAsync(userId);

            return result.Success
                ? $"Success! Beam ID: {result.CreatedObjectGuid}"
                : $"Failed: {result.Message}";
        }

        [McpServerTool, Description("Delete a beam in Tekla")]
        public async Task<string> DeleteBeam()
        {

            var context = _httpContextAccessor.HttpContext;
            string? userId = context?.Request.Query["userId"].ToString();

            if (string.IsNullOrEmpty(userId))
            {
                return "Failed: Unauthorized. No userId was provided by the client.";
            }

            var result = await _modelingService.DeleteBeamAsync(userId);

            return result.Success
                ? $"Success! {result.Message}"
                : $"Failed: {result.Message}";
        }
        [McpServerTool, Description("Finds all beams with a specific profile and updates their Class attribute.")]
        public async Task<string> UpdateBeamClassByProfile(
            [Description("The profile string to filter for, e.g., 'ISMB300'")] string profileName,
            [Description("The new class number to assign, e.g., 5")] int newClass)
        {

            var context = _httpContextAccessor.HttpContext;
            string? userId = context?.Request.Query["userId"].ToString();

            if (string.IsNullOrEmpty(userId))
            {
                return "Failed: Unauthorized. No userId was provided by the client.";
            }

            var result = await _modelingService.UpdateBeamClassByProfileAsync(userId, profileName, newClass);

            return result.Success
                ? $"Success! {result.Message}"
                : $"Failed: {result.Message}";
        }

    }
}
