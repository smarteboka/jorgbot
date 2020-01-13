using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Options;

namespace Smartbot.Data.Storage.Events
{
    public class EventsStorage : IEventsStorage
    {
        private readonly CloudTable _eventsTable;

        public EventsStorage(IOptions<SmartStorageOptions> options)
            : this(options.Value.Smartbot_AzureStorage_AccountKey)
        {
        }

        public EventsStorage(string accountKey)
        {
            var connectionString = $"DefaultEndpointsProtocol=https;AccountName=smartestorage;AccountKey={accountKey};EndpointSuffix=core.windows.net";
            var account = CloudStorageAccount.Parse(connectionString);
            var client = account.CreateCloudTableClient(new TableClientConfiguration());
            _eventsTable = client.GetTableReference("Events");
        }

        public async Task<EventEntity> Save(EventEntity entity)
        {
            EventEntity insertedUrl;
            try
            {
                var insert = TableOperation.Insert(entity);
                var result = await _eventsTable.ExecuteAsync(insert);
                insertedUrl = result.Result as EventEntity;
            }
            catch (StorageException e)
            {
                throw new SmartStorageException(e.Message);
            }
            return insertedUrl;
        }

        public async Task<List<EventEntity>> GetEventsInRange(string eventType, DateTime from, DateTime to)
        {
            TableQuerySegment<EventEntity> result;
            try
            {
                var afterFrom = TableQuery.GenerateFilterConditionForDate(nameof(EventEntity.EventTime), QueryComparisons.GreaterThanOrEqual, @from.Date);
                var beforeTo = TableQuery.GenerateFilterConditionForDate(nameof(EventEntity.EventTime), QueryComparisons.LessThan, to.Date);
                var inDateRange = $"{afterFrom} {TableOperators.And} {beforeTo}";
                var query = new TableQuery<EventEntity>()
                    .Where(
                        TableQuery.CombineFilters(
                            TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, eventType),
                            TableOperators.And,
                            inDateRange));

                result = await _eventsTable.ExecuteQuerySegmentedAsync(query, null);
            }
            catch (StorageException e)
            {
                throw new SmartStorageException(e.Message);
            }
            return result.Results;
        }

        public async Task<bool> DeleteAll(string eventType)
        {
            var allEventsByType = await GetEventsInRange(eventType,new DateTime(2010,1,1), new DateTime(2090,1,1));
            TableBatchResult result;
            try
            {
                var batch = new TableBatchOperation();
                foreach (var m in allEventsByType)
                {
                    batch.Add(TableOperation.Delete(m));
                }

                result = await _eventsTable.ExecuteBatchAsync(batch);
            }
            catch (StorageException e)
            {
                throw new SmartStorageException(e.Message);
            }

            return result.All(r => r.HttpStatusCode == 204);
        }

        public async Task<EventEntity> GetNextEvent(string eventType, DateTime? fromDate = null)
        {
            fromDate = fromDate ?? DateTime.UtcNow;
            var eventsNext5Years = await GetEventsInRange(eventType, fromDate.Value, fromDate.Value.AddYears(5));
            return eventsNext5Years.OrderBy(e => e.EventTime).FirstOrDefault();
        }
    }
}