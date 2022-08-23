namespace TrfrtSbmt.Api.Features;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using TrfrtSbmt.Api.DataModels;

public class DynamoDbQueries
{
    public abstract class BaseDynamoQuery
    {
        protected readonly IAmazonDynamoDB _db;
        protected readonly AppSettings _settings;

        public BaseDynamoQuery(IAmazonDynamoDB db, AppSettings settings)
        {
            _db = db;
            _settings = settings;
        }


    }

    public class SearchTermQuery : BaseDynamoQuery
    {
        private const string SearchTermIndex = "SearchTermIndex";

        public SearchTermQuery(IAmazonDynamoDB db, AppSettings settings) : base(db, settings) { }

        public async Task<QueryResponse> ExecuteAsync(string searchTerm, int pageSize, Dictionary<string, AttributeValue>? exclusiveStartKey)
        {
            var pkSymbol = ":partitionKey";
            return await _db.QueryAsync(new QueryRequest(_settings.TableName)
            {
                IndexName = SearchTermIndex,
                KeyConditionExpression = $"{nameof(BaseEntity.SearchTerm)} = {pkSymbol}",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    [pkSymbol] = new AttributeValue { S = searchTerm.ToUpperInvariant() }
                },
                Limit = pageSize,
                ExclusiveStartKey = exclusiveStartKey
            });
        }

        public async Task<QueryResponse> ExecuteAsync(string searchTerm, string type, int pageSize, Dictionary<string, AttributeValue>? exclusiveStartKey)
        {
            // $"{nameof(BaseEntity.PartitionKey)} = :pk AND {nameof(BaseEntity.SortKey)} = :sk"
            var pkSymbol = ":partitionKey";
            var skSymbol = ":sortKey";
            return await _db.QueryAsync(new QueryRequest(_settings.TableName)
            {
                IndexName = SearchTermIndex,
                KeyConditionExpression = $"{nameof(BaseEntity.SearchTerm)} = {pkSymbol} AND {nameof(BaseEntity.EntityType)} = {skSymbol}",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    [pkSymbol] = new AttributeValue { S = searchTerm.ToUpperInvariant() },
                    [skSymbol] = new AttributeValue { S = type }
                },
                Limit = pageSize,
                ExclusiveStartKey = exclusiveStartKey
            });
        }
    }

    public class EntityIdQuery : BaseDynamoQuery
    {
        private const string EntityIdIndex = "EntityIdIndex";

        public EntityIdQuery(IAmazonDynamoDB db, AppSettings settings) : base(db, settings) { }

        public async Task<QueryResponse> ExecuteAsync(string id, int pageSize, Dictionary<string, AttributeValue>? exclusiveStartKey)
        {
            return await _db.QueryAsync(new QueryRequest(_settings.TableName)
            {
                KeyConditionExpression = $"{nameof(BaseEntity.EntityId)} = :id",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
                {
                    {":id", new AttributeValue(id)}
                },
                IndexName = EntityIdIndex,
                Limit = pageSize,
                ExclusiveStartKey = exclusiveStartKey
            });
        }
    }

    public class BeginsWithQuery : BaseDynamoQuery
    {
        public BeginsWithQuery(IAmazonDynamoDB db, AppSettings settings) : base(db, settings) { }

        public async Task<QueryResponse> ExecuteAsync(string pk, string sk, int pageSize, Dictionary<string, AttributeValue>? exclusiveStartKey)
        {
            var pkSymbol = ":pk";
            var skSymbol = ":sk";

            return await _db.QueryAsync(new QueryRequest(_settings.TableName)
            {
                KeyConditionExpression = $"{nameof(BaseEntity.PartitionKey)} = {pkSymbol} and begins_with({nameof(BaseEntity.SortKey)}, {skSymbol})",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { pkSymbol, new AttributeValue(pk) },
                    { skSymbol, new AttributeValue(sk) }
                },
                ExclusiveStartKey = exclusiveStartKey,
                Limit = pageSize
            });
        }
    }


    public class Query : BaseDynamoQuery
    {
        public Query(IAmazonDynamoDB db, AppSettings settings) : base(db, settings) { }

        public async Task<QueryResponse> ExecuteAsync(string partitionKey, Dictionary<string, AttributeValue>? exclusiveStartKey = null)
        {
            return await _db.QueryAsync(new QueryRequest(_settings.TableName)
            {
                KeyConditionExpression = $"{nameof(BaseEntity.PartitionKey)} = :pk",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
                {
                    {":pk", new AttributeValue(partitionKey)}
                },
                ExclusiveStartKey = exclusiveStartKey
            });
        }
        public async Task<QueryResponse> ExecuteAsync(string partitionKey, string sortKey)
        {
            return await _db.QueryAsync(new QueryRequest(_settings.TableName)
            {
                KeyConditionExpression = $"{nameof(BaseEntity.PartitionKey)} = :pk AND {nameof(BaseEntity.SortKey)} = :sk",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
                {
                    {":pk", new AttributeValue(partitionKey)},
                    {":sk", new AttributeValue(sortKey)}
                }
            });
        }
    }

    public class CreatedByQuery : BaseDynamoQuery
    {
        public CreatedByQuery(IAmazonDynamoDB db, AppSettings settings) : base(db, settings) { }

        public async Task<QueryResponse> ExecuteAsync(string createdBy)
        {
            return await _db.QueryAsync(new QueryRequest(_settings.TableName)
            {
                KeyConditionExpression = $"{nameof(BaseEntity.CreatedBy)} = :pk",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
                    {
                        {":pk", new AttributeValue(createdBy)}
                    },
                IndexName = "CreatedByIndex"
            });
        }
    }

    public class DeleteBatch : BaseDynamoQuery
    {
        public DeleteBatch(IAmazonDynamoDB db, AppSettings settings) : base(db, settings)
        {
        }

        public async Task ExecuteAsync(List<Dictionary<string, AttributeValue>> items)
        {
            // batch delete the items
            // group items into lists of 25        
            var batches = items.Chunk(25);
            foreach (var batch in batches)
            {
                await _db.BatchWriteItemAsync(new BatchWriteItemRequest
                {
                    RequestItems =
                    {
                        [_settings.TableName] = batch.Select(item => new WriteRequest
                        {
                            DeleteRequest = new DeleteRequest
                            {
                                Key = new Dictionary<string, AttributeValue>()
                                {
                                    [nameof(BaseEntity.PartitionKey)] = new AttributeValue(item[nameof(BaseEntity.PartitionKey)].S),
                                    [nameof(BaseEntity.SortKey)] = new AttributeValue(item[nameof(BaseEntity.SortKey)].S)
                                }
                            }
                        }).ToList()
                    }
                });
            }
        }
    }
}


