using Amazon.CDK.AWS.DynamoDB;
using Attribute = Amazon.CDK.AWS.DynamoDB.Attribute;

namespace TrfrtSbmt.Cdk.Constructs
{
    public class SubmissionsTableProps
    {
        public string EnvironmentName { get; set; } = "Development";
        public string TableName { get; set; } = "Submissions";
        public RemovalPolicy RemovalPolicy { get; set; }
    }
    public class SubmissionsTable : Construct
    {
        public Table Table { get; }
        public SubmissionsTable(Construct scope, string id, SubmissionsTableProps props) : base(scope, id)
        {
            Table = new Table(this, id, new TableProps
            {
                BillingMode = BillingMode.PAY_PER_REQUEST,
                PartitionKey = new Attribute
                {
                    Name = "PartitionKey",
                    Type = AttributeType.STRING
                },
                SortKey = new Attribute
                {
                    Name = "SortKey",
                    Type = AttributeType.STRING
                },
                RemovalPolicy = props.RemovalPolicy,
                TableName = $"{props.EnvironmentName}-{props.TableName}",
                Stream = StreamViewType.NEW_IMAGE                
            });

            Tags.Of(Table).Add("Name", $"{props.EnvironmentName}-{props.TableName}");
            Tags.Of(Table).Add("Last Updated", DateTimeOffset.UtcNow.ToString());

            Table.AddLocalSecondaryIndex(new LocalSecondaryIndexProps
            {
                IndexName = "LocationIndex",
                ProjectionType = ProjectionType.ALL,
                SortKey = new Attribute
                {
                    Name = "Location",
                    Type = AttributeType.STRING
                }
            });

            Table.AddLocalSecondaryIndex(new LocalSecondaryIndexProps
            {
                IndexName = "RankIndex",
                ProjectionType = ProjectionType.ALL,
                SortKey = new Attribute
                {
                    Name = "Rank",
                    Type = AttributeType.STRING
                }
            });

            Table.AddLocalSecondaryIndex(new LocalSecondaryIndexProps
            {
                IndexName = "LocationRankIndex",
                ProjectionType = ProjectionType.ALL,
                SortKey = new Attribute
                {
                    Name = "LocationRank",
                    Type = AttributeType.STRING
                }
            });

            Table.AddLocalSecondaryIndex(new LocalSecondaryIndexProps
            {
                IndexName = "SubmissionDateIndex",
                ProjectionType = ProjectionType.ALL,
                SortKey = new Attribute
                {
                    Name = "SubmissionDate",
                    Type = AttributeType.STRING
                }
            });

            Table.AddGlobalSecondaryIndex(new GlobalSecondaryIndexProps
            {
                IndexName = "SearchTermIndex",
                PartitionKey = new Attribute
                {
                    Name = "SearchTerm",
                    Type = AttributeType.STRING
                },
                SortKey = new Attribute
                {
                    Name = "EntityType",
                    Type = AttributeType.STRING
                }
            });

            Table.AddGlobalSecondaryIndex(new GlobalSecondaryIndexProps
            {
                IndexName = "EntityIdIndex",
                PartitionKey = new Attribute
                {
                    Name = "EntityId",
                    Type = AttributeType.STRING
                }
            });

            Table.AddGlobalSecondaryIndex(new GlobalSecondaryIndexProps
            {
                IndexName = "CreatedByIndex",
                PartitionKey = new Attribute
                {
                    Name = "CreatedBy",
                    Type = AttributeType.STRING
                },
                SortKey = new Attribute
                {
                    Name = "EntityType",
                    Type = AttributeType.STRING
                }
            });
        }
    }
}
