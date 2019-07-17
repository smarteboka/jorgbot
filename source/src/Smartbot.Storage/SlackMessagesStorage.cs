using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Options;

namespace Smartbot.Storage
{
    public class SlackMessagesStorage
    {
        private readonly CloudTable _table;

        public SlackMessagesStorage(IOptions<SmartStorageOptions> options) 
            : this(options.Value.Smartbot_AzureStorage_AccountKey)
        {
        }

        public SlackMessagesStorage(string accountKey)
        {
            var connectionString = $"DefaultEndpointsProtocol=https;AccountName=smartestorage;AccountKey={accountKey};EndpointSuffix=core.windows.net";
            var account = CloudStorageAccount.Parse(connectionString);
            var client = account.CreateCloudTableClient(new TableClientConfiguration());
            _table = client.GetTableReference("SlackUrls");
        }

        public async Task<SlackMessageEntity> Save(SlackMessageEntity entity)
        {
            SlackMessageEntity insertedMessage;
            try
            {
                var insert = TableOperation.Insert(entity);
                var result = await _table.ExecuteAsync(insert);
                insertedMessage = result.Result as SlackMessageEntity;
            }
            catch (StorageException e)
            {
                throw new SmartStorageException(e.Message);
            }
            return insertedMessage; 
        }

        public async Task<List<SlackMessageEntity>> GetAllByUser(string user)
        {
            TableQuerySegment<SlackMessageEntity> result;
            try
            {
                var query = new TableQuery<SlackMessageEntity>()
                    .Where(
                        TableQuery.CombineFilters(
                            TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, SlackMessageEntity.EntityPartitionKey),
                            TableOperators.And,
                            TableQuery.GenerateFilterCondition(nameof(SlackMessageEntity.User), QueryComparisons.Equal, user)
                        ));

                result = await _table.ExecuteQuerySegmentedAsync(query, null);
            }
            catch (StorageException e)
            {
                throw new SmartStorageException(e.Message);
            }
            return result.Results;
        }

        public async Task<bool> DeleteAllByUser(string user)
        {
            var allByUser = await GetAllByUser(user);
            TableBatchResult result;
            try
            {
                var batch = new TableBatchOperation();
                foreach (var m in allByUser)
                {
                    batch.Add(TableOperation.Delete(m));   
                }

                result = await _table.ExecuteBatchAsync(batch);
            }
            catch (StorageException e)
            {
                throw new SmartStorageException(e.Message);
            }

            return result.All(r => r.HttpStatusCode == 204);
        }
    }
}