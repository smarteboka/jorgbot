using System;
using Microsoft.Azure.Cosmos.Table;

namespace Smartbot.Utilities.Storage.Events
{
    public class EventEntity : TableEntity
    {
        public EventEntity()
        {

        }

        public EventEntity(Guid id, string eventType)
        {
            PartitionKey = eventType;
            RowKey = id.ToString();
        }

        public string Topic
        {
            get;
            set;
        }

        public DateTimeOffset EventTime
        {
            get;
            set;
        }

        public string Location
        {
            get;
            set;
        }

        public bool Finalized
        {
            get;
            set;
        }
    }
}