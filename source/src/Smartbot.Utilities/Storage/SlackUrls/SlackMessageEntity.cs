using System;
using Microsoft.Azure.Cosmos.Table;

namespace Smartbot.Utilities.Storage.SlackUrls
{
    public class SlackMessageEntity : TableEntity
    {
        public const string EntityPartitionKey = "SlackMessage";

        public SlackMessageEntity()
        {
            
        }
        
        public SlackMessageEntity(Guid id)
        {
            PartitionKey = EntityPartitionKey;
            RowKey = id.ToString();
        }

        public string Text
        {
            get;
            set;
        }

        public string User
        {
            get;
            set;
        }

        public string Raw
        {
            get;
            set;
        }

        public string ChannelName
        {
            get;
            set;
        }
       
        public string SlackTimestamp
        {
            get;
            set;
        }

        public string Permalink
        {
            get;
            set;
        }
    }
}