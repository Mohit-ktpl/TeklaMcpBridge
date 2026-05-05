using Contracts.Models;

namespace Server.Services.Interface
{
    public interface IModelingService
    {
        Task<SharedResult> DesignAndCreateBeamAsync(string userId, double span, double load);
        Task<SharedResult> CreateSimpleBeamAsync(string userId);
        Task<SharedResult> DeleteBeamAsync(string userId);
        Task<SharedResult> UpdateBeamClassByProfileAsync(string userId, string profileName, int newClass);

    }
}
