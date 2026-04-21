namespace Server.Services.Interface
{
    public interface IUserTracker
    {
        void Add(string userId,  string connectionId);
        void Remove(string connectionId);

        string GetConnectionId(string userId);
    }
}
