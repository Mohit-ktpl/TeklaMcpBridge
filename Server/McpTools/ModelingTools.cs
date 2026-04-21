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

        public ModelingTools(IModelingService modelingService)
        {
            _modelingService = modelingService;
        }
        [McpServerTool, Description("Designs and creates a beam in Tekla based on span and load")]
        public async Task<string> CreateBeam( double span, double load)
        {
            string userId = "dev_user";
            var result = await _modelingService.DesignAndCreateBeamAsync(userId, span, load);
            return result.Success ? $"Success! Beam ID: {result.CreatedObjectGuid}" : $"Failed: {result.Message}";
        }
    }
}
