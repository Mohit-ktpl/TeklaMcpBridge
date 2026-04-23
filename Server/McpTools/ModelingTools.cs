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
        //[McpServerTool, Description( "Tests if the bridge is stable")]
        //public async Task<string> TestConnection()
        //{
        //    return "The bridge is stable!";
        //}
    }
}
