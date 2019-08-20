using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Options;

namespace Smartbot.Utilities.Storage.Events
{
    public class InvitationsStorage : IInvitationsStorage
    {
        private readonly CloudTable _invitationsTable;

        public InvitationsStorage(IOptions<SmartStorageOptions> options)
            : this(options.Value.Smartbot_AzureStorage_AccountKey)
        {
        }

        public InvitationsStorage(string accountKey)
        {
            var connectionString = $"DefaultEndpointsProtocol=https;AccountName=smartestorage;AccountKey={accountKey};EndpointSuffix=core.windows.net";
            var account = CloudStorageAccount.Parse(connectionString);
            var client = account.CreateCloudTableClient(new TableClientConfiguration());
            _invitationsTable = client.GetTableReference("Invitations");
        }

        public async Task<InvitationsEntity> Save(InvitationsEntity invite)
        {
            try
            {
                var insert = TableOperation.Insert(invite);
                var result = await _invitationsTable.ExecuteAsync(insert);
                invite = result.Result as InvitationsEntity;
            }
            catch (StorageException e)
            {
                throw new SmartStorageException(e.Message);
            }
            return invite;
        }

        public async Task<InvitationsEntity> Update(string inviteId, string rsvp)
        {
            var toBeReplaced = await GetById(inviteId);

            try
            {
                toBeReplaced.Rsvp = rsvp;
                var replace = TableOperation.Replace(toBeReplaced);
                var result = await _invitationsTable.ExecuteAsync(replace);
                toBeReplaced = result.Result as InvitationsEntity;
            }
            catch (StorageException e)
            {
                throw new SmartStorageException(e.Message);
            }
            return toBeReplaced;
        }

        public async Task<InvitationsEntity> GetById(string inviteId)
        {
            try
            {
                var operation = TableOperation.Retrieve<InvitationsEntity>(InvitationsEntity.EntityPartitionKey, inviteId);
                var result = await _invitationsTable.ExecuteAsync(operation);
                return result.Result as InvitationsEntity;
            }
            catch (StorageException e)
            {
                throw new SmartStorageException(e.Message);
            }
        }

        public async Task<IEnumerable<InvitationsEntity>> GetInvitations(string eventId)
        {
            try
            {
                var filters = TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, InvitationsEntity.EntityPartitionKey),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition(nameof(InvitationsEntity.EventId), QueryComparisons.Equal, eventId.ToString()));


                var query = new TableQuery<InvitationsEntity>().Where(filters);

                var result = await _invitationsTable.ExecuteQuerySegmentedAsync(query,null);
                return result.Results;
            }
            catch (StorageException e)
            {
                throw new SmartStorageException(e.Message);
            }
        }

        public async Task<List<EventEntity>> GetInvitations()
        {
            TableQuerySegment<EventEntity> result;
            try
            {
                var partitionKey = TableQuery.GenerateFilterCondition("PartitionKey",
                    QueryComparisons.Equal,
                    InvitationsEntity.EntityPartitionKey);

                var query = new TableQuery<EventEntity>().Where(partitionKey);

                result = await _invitationsTable.ExecuteQuerySegmentedAsync(query, null);
            }
            catch (StorageException e)
            {
                throw new SmartStorageException(e.Message);
            }
            return result.Results;
        }
    }
}