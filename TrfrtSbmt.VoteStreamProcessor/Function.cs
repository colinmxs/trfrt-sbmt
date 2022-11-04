using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using Amazon.DynamoDBv2.Model;
using TrfrtSbmt.Domain;
using Amazon.DynamoDBv2;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace TrfrtSbmt.VoteStreamProcessor;

public class Function
{
    public async Task FunctionHandler(DynamoDBEvent dynamoEvent, ILambdaContext context)
    {
        // get environment variable
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var tableName = $"Submissions-{environment}";
        var dbClient = new AmazonDynamoDBClient();
        context.Logger.LogInformation($"Beginning to process {dynamoEvent.Records.Count} records...");

        foreach (var record in dynamoEvent.Records)
        {
            context.Logger.LogInformation($"Event ID: {record.EventID}");
            context.Logger.LogInformation($"Event Name: {record.EventName}");

            Dictionary<string, AttributeValue> image = new Dictionary<string, AttributeValue>();
            image = record.EventName == "REMOVE" ? record.Dynamodb.OldImage : record.Dynamodb.NewImage;
            if (image[nameof(BaseEntity.EntityType)].S == nameof(Vote))
            {
                var vote = new Vote(image);
                var submissionId = vote.PartitionKey;
                List<Dictionary<string, AttributeValue>> items = new();
                var queryResponse = await dbClient.QueryAsync(new QueryRequest(tableName)
                {
                    IndexName = BaseEntity.EntityIdIndex,
                    KeyConditionExpression = $"{nameof(BaseEntity.EntityId)} = :id",
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
                    {
                        {":id", new AttributeValue(submissionId)}
                    }
                });

                var submission = queryResponse.Items.SingleOrDefault(i => i["EntityId"].S == submissionId && i["SortKey"].S.StartsWith(nameof(Submission) + "-"));


                queryResponse = await dbClient.QueryAsync(new QueryRequest(tableName)
                {
                    KeyConditionExpression = $"{nameof(BaseEntity.PartitionKey)} = :pk",
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
                    {
                        {":pk", new AttributeValue(submissionId)}
                    }
                });
                items.AddRange(queryResponse.Items);

                while (queryResponse.LastEvaluatedKey.Any())
                {
                    queryResponse = await dbClient.QueryAsync(new QueryRequest(tableName)
                    {
                        KeyConditionExpression = $"{nameof(BaseEntity.PartitionKey)} = :pk",
                        ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
                        {
                            {":pk", new AttributeValue(submissionId)}
                        },
                        ExclusiveStartKey = queryResponse.LastEvaluatedKey
                    }); 
                    items.AddRange(queryResponse.Items);
                }
                
                var votes = items.Where(i => i["SortKey"].S.StartsWith(nameof(Vote) + "-")).Select(i => new Vote(i)).ToList();

                if (submission == null) continue;
                var submissionModel = new Submission(submission);
                decimal rank = Decimal.Divide(votes.Sum(v => v.Value), votes.Count());
                var submissionRank = new SubmissionRank(submissionModel, Math.Round(rank, 2), votes.Count());
                await dbClient.PutItemAsync(new PutItemRequest(tableName, submissionRank.ToDictionary()));
            }
        }

        context.Logger.LogInformation("Stream processing complete.");
    }
}