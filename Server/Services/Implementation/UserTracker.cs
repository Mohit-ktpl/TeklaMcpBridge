using Server.Services.Interface;
using System.Collections.Concurrent;

namespace Server.Services.Implementation
{
    public class UserTracker : IUserTracker
    {
        private readonly ConcurrentDictionary<string, string> _userConnections = new ConcurrentDictionary<string, string>();
        void IUserTracker.Add(string userId, string connectionId)
        {
            _userConnections[userId] = connectionId;
        }

        string IUserTracker.GetConnectionId(string userId)
        {
           _userConnections.TryGetValue(userId, out var connectionId);
            return connectionId;
        }

        void IUserTracker.Remove(string connectionId)
        {
            var item = _userConnections.FirstOrDefault(kvp => kvp.Value == connectionId);
            if (item.Key != null)
            {
                _userConnections.TryRemove(item.Key, out _);
            }
        }

    }
}
