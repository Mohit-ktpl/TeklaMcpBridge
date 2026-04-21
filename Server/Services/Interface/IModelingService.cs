using Contracts.Models;

namespace Server.Services.Interface
{
    public interface IModelingService
    {
        Task<SharedResult> DesignAndCreateBeamAsync(string userId, double span, double load);
    }
}
