using System.Collections.Generic;

namespace PocketDungeons.Core.Services
{
    public interface IAnalyticsService
    {
        void LogEvent(string eventName, Dictionary<string, object> parameters = null);
        void SetUserProperty(string name, string value);
        void SetUserId(string userId);
    }
}
