namespace TrfrtSbmt.Cdk;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Route53;
using Amazon.CDK.AWS.Route53.Targets;

public class ApiStack : Stack
{
    public class ApiStackProps : StackProps
    {
        public string Name { get; init; } = "TreefortSubmitApi";
        public string Region { get; init; } = "us-east-1";
        public string[] OtherRegions { get; init; } = new string[] { "us-west-2" };
    }
    
    public ApiStack(Construct scope, string id, ApiStackProps props) : base(scope, id, props)
    {
        var accountId = (string)scope.Node.TryGetContext("accountid");
        var sslCertId = (string)scope.Node.TryGetContext("sslcertid");
        

        var lambdaExecutionRole = new Role(this, "ApiLambdaExecutionRole", new RoleProps
        {
            AssumedBy = new ServicePrincipal("lambda.amazonaws.com"),
            RoleName = $"{props.Name}LambdaExecutionRole",
            InlinePolicies = new Dictionary<string, PolicyDocument>
            {
                {
                    "cloudwatch-policy",
                    new PolicyDocument(
                        new PolicyDocumentProps {
                            AssignSids = true,
                            Statements = new [] {
                                new PolicyStatement(new PolicyStatementProps {
                                    Effect = Effect.ALLOW,
                                    Actions = new string[] {
                                        "logs:CreateLogStream",
                                        "logs:PutLogEvents",
                                        "logs:CreateLogGroup"
                                    },
                                    Resources = new string[] {
                                        "arn:aws:logs:*:*:*"
                                    }
                                })
                            }
                        })
                }
            }
        });
        Amazon.CDK.Tags.Of(lambdaExecutionRole).Add("Name", $"{props.Name}LambdaExecutionRole");
        Amazon.CDK.Tags.Of(lambdaExecutionRole).Add("Last Updated", DateTimeOffset.UtcNow.ToString());

        var lambdaFunction = new Function(this, "LambdaFunction", new FunctionProps
        {
            Code = new AssetCode($"{Utilities.GetDirectory("TrfrtSbmt.Api")}/publish.zip"),
            Handler = "TrfrtSbmt.Api",
            Runtime = Runtime.DOTNET_6,
            Timeout = Duration.Seconds(10),
            FunctionName = $"{props.Name}LambdaFunction",            
            MemorySize = 256,
            RetryAttempts = 1,
            Role = lambdaExecutionRole,
            Environment = new Dictionary<string, string>
            {
                ["ASPNETCORE_ENVIRONMENT"] = "Production" 
            }
        });
        Amazon.CDK.Tags.Of(lambdaFunction).Add("Name", $"{props.Name}LambdaFunction");
        Amazon.CDK.Tags.Of(lambdaExecutionRole).Add("Last Updated", DateTimeOffset.UtcNow.ToString());

        var restApi = new LambdaRestApi(this, "RestApi", new LambdaRestApiProps
        {
            DeployOptions = new StageOptions
            {
                StageName = "v1"
            },
            Handler = lambdaFunction,
            Proxy = true,
            Deploy = true,
            DefaultCorsPreflightOptions = new CorsOptions 
            {
                AllowOrigins = Cors.ALL_ORIGINS,
                AllowMethods = Cors.ALL_METHODS,
                AllowHeaders = Cors.DEFAULT_HEADERS
            },
            RestApiName = $"{props.Name}RestApi",
            EndpointConfiguration = new EndpointConfiguration
            {
                Types = new EndpointType[]
                {
                    EndpointType.EDGE
                }
            },
            DomainName = new DomainNameOptions
            {
                Certificate = Certificate.FromCertificateArn(this, "uswest2privatecert", $"arn:aws:acm:us-east-1:{accountId}:certificate/{sslCertId}"),
                DomainName = "sbmt-api.com",
                EndpointType = EndpointType.EDGE,
                SecurityPolicy = SecurityPolicy.TLS_1_2
            }
        });
        Amazon.CDK.Tags.Of(restApi).Add("Name", $"{props.Name}RestApi");
        Amazon.CDK.Tags.Of(lambdaExecutionRole).Add("Last Updated", DateTimeOffset.UtcNow.ToString());

        var table = new Table(this, "DynamoTable", new TableProps
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
            RemovalPolicy = RemovalPolicy.DESTROY,
            TableName = $"Submissions",
            ReplicationRegions = props.OtherRegions
        });

        Amazon.CDK.Tags.Of(table).Add("Name", "Submissions");
        Amazon.CDK.Tags.Of(table).Add("Last Updated", DateTimeOffset.UtcNow.ToString());

        var route53 = new RecordSet(this, "customdomain", new RecordSetProps
        {
            RecordName = "sbmt-api.com",
            RecordType = RecordType.A,
            Zone = HostedZone.FromLookup(this, "HostedZone", new HostedZoneProviderProps
            {
                DomainName = "sbmt-api.com"
            }),
            Target = RecordTarget.FromAlias(new ApiGateway(restApi))
        });

        Amazon.CDK.Tags.Of(route53).Add("Name", "sbmt-api.com");
        Amazon.CDK.Tags.Of(route53).Add("Last Updated", DateTimeOffset.UtcNow.ToString());
    }
}
