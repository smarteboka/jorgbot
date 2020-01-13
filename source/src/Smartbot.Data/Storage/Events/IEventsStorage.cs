using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Smartbot.Data.Storage.Events
{
    public interface IEventsStorage
    {
        Task<EventEntity> Save(EventEntity entity);
        Task<List<EventEntity>> GetEventsInRange(string eventType, DateTime from, DateTime to);
        Task<bool> DeleteAll(string eventType);
        Task<EventEntity> GetNextEvent(string eventType, DateTime? fromDate = null);
    }
}