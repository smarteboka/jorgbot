using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Options;

namespace Smartbot.Utilities.Storage.SlackUrls
{
    public class SlackMessagesStorage
    {
        private readonly CloudTable _slackUrlsTable;
        private readonly CloudTable _slackMessagesTable;

        public SlackMessagesStorage(IOptions<SmartStorageOptions> options) 
            : this(options.Value.Smartbot_AzureStorage_AccountKey)
        {
        }

        public SlackMessagesStorage(string accountKey)
        {
            var connectionString = $"DefaultEndpointsProtocol=https;AccountName=smartestorage;AccountKey={accountKey};EndpointSuffix=core.windows.net";
            var account = CloudStorageAccount.Parse(connectionString);
            var client = account.CreateCloudTableClient(new TableClientConfiguration());
            _slackUrlsTable = client.GetTableReference("SlackUrls");
            _slackMessagesTable = client.GetTableReference("SlackMessages");
        }

        public async Task<SlackUrlEntity> Save(SlackUrlEntity entity)
        {
            SlackUrlEntity insertedUrl;
            try
            {
                var insert = TableOperation.Insert(entity);
                var result = await _slackUrlsTable.ExecuteAsync(insert);
                insertedUrl = result.Result as SlackUrlEntity;
            }
            catch (StorageException e)
            {
                throw new SmartStorageException(e.Message);
            }
            return insertedUrl; 
        }
        
        public async Task<SlackMessageEntity> Save(SlackMessageEntity entity)
        {
            SlackMessageEntity insertedUrl;
            try
            {
                var insert = TableOperation.Insert(entity);
                var result = await _slackMessagesTable.ExecuteAsync(insert);
                insertedUrl = result.Result as SlackMessageEntity;
            }
            catch (StorageException e)
            {
                throw new SmartStorageException(e.Message);
            }
            return insertedUrl; 
        }

        public async Task<List<SlackUrlEntity>> GetAllUrlsByUser(string user)
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
                        )).OrderByDesc(nameof(SlackUrlEntity.Timestamp));

                result = await _slackUrlsTable.ExecuteQuerySegmentedAsync(query, null);
            }
            catch (StorageException e)
            {
                throw new SmartStorageException(e.Message);
            }
            return result.Results;
        }

        public async Task<bool> DeleteAllUrlsByUser(string user)
        {
            var allByUser = await GetAllUrlsByUser(user);
            TableBatchResult result;
            try
            {
                var batch = new TableBatchOperation();
                foreach (var m in allByUser)
                {
                    batch.Add(TableOperation.Delete(m));   
                }

                result = await _slackUrlsTable.ExecuteBatchAsync(batch);
            }
            catch (StorageException e)
            {
                throw new SmartStorageException(e.Message);
            }

            return result.All(r => r.HttpStatusCode == 204);
        }

        public async Task<IEnumerable<SlackMessageEntity>> GetAllMessagesByUser(string user)
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

                result = await _slackMessagesTable.ExecuteQuerySegmentedAsync(query, null);
            }
            catch (StorageException e)
            {
                throw new SmartStorageException(e.Message);
            }
            return result.Results;        
        }

        public async Task<bool> DeleteAllMessagesByUser(string user)
        {
         var allByUser = await GetAllMessagesByUser(user);
            TableBatchResult result;
            try
            {
                var batch = new TableBatchOperation();
                foreach (var m in allByUser)
                {
                    batch.Add(TableOperation.Delete(m));   
                }

                result = await _slackMessagesTable.ExecuteBatchAsync(batch);
            }
            catch (StorageException e)
            {
                throw new SmartStorageException(e.Message);
            }

            return result.All(r => r.HttpStatusCode == 204);        
        }

        public async Task<bool> DeleteMessage(SlackUrlEntity entity)
        {
            var res = await _slackUrlsTable.ExecuteAsync(TableOperation.Delete(entity));
            return res.HttpStatusCode == 204;
        }

        public async Task<IEnumerable<string>> GetUniqueUsersForUrls()
        {
            var query = new TableQuery<SlackUrlEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, SlackUrlEntity.EntityPartitionKey));
            
            
            var tableResult = await _slackUrlsTable.ExecuteQuerySegmentedAsync(query, null);
            return tableResult.Results.Select(u => u.User).Distinct();
        }
    }
}