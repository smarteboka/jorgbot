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

        public async Task<SlackUrlEntity> Save(SlackUrlEntity entity)
        {
            SlackUrlEntity insertedUrl;
            try
            {
                var insert = TableOperation.Insert(entity);
                var result = await _table.ExecuteAsync(insert);
                insertedUrl = result.Result as SlackUrlEntity;
            }
            catch (StorageException e)
            {
                throw new SmartStorageException(e.Message);
            }
            return insertedUrl; 
        }

        public async Task<List<SlackUrlEntity>> GetAllByUser(string user)
        {
            TableQuerySegment<SlackUrlEntity> result;
            try
            {
                var query = new TableQuery<SlackUrlEntity>()
                    .Where(
                        TableQuery.CombineFilters(
                            TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, SlackUrlEntity.EntityPartitionKey),
                            TableOperators.And,
                            TableQuery.GenerateFilterCondition(nameof(SlackUrlEntity.User), QueryComparisons.Equal, user)
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