using System;
using Microsoft.Azure.Cosmos.Table;

namespace Smartbot.Utilities.Storage.Events
{
    public class InvitationsEntity : TableEntity
    {
        public const string EntityPartitionKey = "Invitation";

        public InvitationsEntity()
        {

        }

        public InvitationsEntity(Guid id)
        {
            PartitionKey = EntityPartitionKey;
            RowKey = id.ToString();
        }

        public string EventTopic
        {
            get;
            set;
        }

        public DateTimeOffset EventTime
        {
            get;
            set;
        }

        public string SlackUserId
        {
            get;
            set;
        }

        public string Rsvp
        {
            get;
            set;
        }

        public string EventId
        {
            get;
            set;
        }
    }
}