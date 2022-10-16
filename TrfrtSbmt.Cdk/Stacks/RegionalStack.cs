namespace TrfrtSbmt.Cdk.Stacks;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Route53;
using Amazon.CDK.AWS.Route53.Targets;
using Amazon.CDK.AWS.S3;

public class RegionalStack : Stack
{
    public class RegionalStackProps : StackProps
    {
        public string EnvironmentName { get; init; } = "Development";
        public string EnvironmentPrefix { get; init; } = "Development-";
        public string Name { get; init; } = "TreefortSubmitApi";
        public string Region { get; init; } = "us-east-1";
        public string CertId { get; init; } = string.Empty;
        //public string[] OtherRegions { get; init; } = new string[] { "us-west-2" };
    }
    
    public RegionalStack(Construct scope, string id, RegionalStackProps props) : base(scope, id, props)
    {
        Amazon.CDK.Tags.Of(this).Add("Billing", "Treefort");
        var accountId = (string)scope.Node.TryGetContext("accountid");
        var domain = (string)scope.Node.TryGetContext("domain");
        var subdomain = (string)scope.Node.TryGetContext("subdomain");

        var lambdaExecutionRole = new Role(this, "ApiLambdaExecutionRole", new RoleProps
        {
            AssumedBy = new ServicePrincipal("lambda.amazonaws.com"),
            RoleName = $"{props.EnvironmentPrefix}{props.Name}LambdaExecutionRole",
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
        Amazon.CDK.Tags.Of(lambdaExecutionRole).Add("Name", $"{props.EnvironmentPrefix}{props.Name}LambdaExecutionRole");
        Amazon.CDK.Tags.Of(lambdaExecutionRole).Add("Last Updated", DateTimeOffset.UtcNow.ToString());

        var lambdaFunction = new Function(this, "LambdaFunction", new FunctionProps
        {
            Code = new AssetCode($"{Utilities.GetDirectory("TrfrtSbmt.Api")}"),
            Handler = "TrfrtSbmt.Api",
            Runtime = Runtime.DOTNET_6,
            Timeout = Duration.Seconds(10),
            FunctionName = $"{props.EnvironmentPrefix}{props.Name}LambdaFunction",
            MemorySize = 2048,
            RetryAttempts = 1,
            Role = lambdaExecutionRole,
            Environment = new Dictionary<string, string>
            {
                ["ASPNETCORE_ENVIRONMENT"] = props.EnvironmentName
            }
        });
        Amazon.CDK.Tags.Of(lambdaFunction).Add("Name", $"{props.EnvironmentPrefix}{props.Name}LambdaFunction");
        Amazon.CDK.Tags.Of(lambdaFunction).Add("Last Updated", DateTimeOffset.UtcNow.ToString());

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
            RestApiName = $"{props.EnvironmentPrefix}{props.Name}RestApi",
            EndpointTypes = new EndpointType[]
            {
                EndpointType.REGIONAL
            },
            DomainName = new DomainNameOptions
            {
                Certificate = Certificate.FromCertificateArn(this, "cert", $"arn:aws:acm:{props.Region}:{accountId}:certificate/{props.CertId}"),
                DomainName = $"{props.EnvironmentPrefix.ToLower()}{subdomain}.{domain}",
                EndpointType = EndpointType.REGIONAL,
                SecurityPolicy = SecurityPolicy.TLS_1_2
            }
        });
        Amazon.CDK.Tags.Of(restApi).Add("Name", $"{props.EnvironmentPrefix.ToLower()}{subdomain}.{domain}");
        Amazon.CDK.Tags.Of(restApi).Add("Last Updated", DateTimeOffset.UtcNow.ToString());

        var route53 = new RecordSet(this, "customdomain", new RecordSetProps
        {
            RecordName = $"{props.EnvironmentPrefix.ToLower()}{subdomain}.{domain}",
            RecordType = RecordType.A,
            Zone = HostedZone.FromLookup(this, "HostedZone", new HostedZoneProviderProps
            {
                DomainName = domain
            }),
            Target = RecordTarget.FromAlias(new ApiGateway(restApi))
        });

        Amazon.CDK.Tags.Of(route53).Add("Name", domain);
        Amazon.CDK.Tags.Of(route53).Add("Last Updated", DateTimeOffset.UtcNow.ToString());

        var table = new SubmissionsTable(this, "DynamoTable", new SubmissionsTableProps
        {
            EnvironmentName = props.EnvironmentName,
            RemovalPolicy = RemovalPolicy.RETAIN,
            TableName = "Submissions"
        }).Table;

        lambdaExecutionRole = new Role(this, "ApiLambdaExecutionRole", new RoleProps
        {
            AssumedBy = new ServicePrincipal("lambda.amazonaws.com"),
            RoleName = $"{props.EnvironmentPrefix}{props.Name}LambdaExecutionRole",
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
        Amazon.CDK.Tags.Of(lambdaExecutionRole).Add("Name", $"{props.EnvironmentPrefix}{props.Name}LambdaExecutionRole");
        Amazon.CDK.Tags.Of(lambdaExecutionRole).Add("Last Updated", DateTimeOffset.UtcNow.ToString());

        var targetFunction = new Function(this, "VoteStream.Function", new FunctionProps
        {
            Runtime = Runtime.DOTNET_6,
            Code = new AssetCode($"{Utilities.GetDirectory("TrfrtSbmt.VoteStreamProcessor")}"),
            Handler = "TrfrtSbmt.VoteStreamProcessor",
            Timeout = Duration.Seconds(10),
            FunctionName = $"{props.EnvironmentPrefix}{props.Name}LambdaFunction",
            MemorySize = 2048,
            RetryAttempts = 1,
            Role = lambdaExecutionRole,
            Environment = new Dictionary<string, string>
            {
                ["ASPNETCORE_ENVIRONMENT"] = props.EnvironmentName
            }
        });
        Amazon.CDK.Tags.Of(targetFunction).Add("Name", $"{props.EnvironmentPrefix}{props.Name}LambdaFunction");
        Amazon.CDK.Tags.Of(targetFunction).Add("Last Updated", DateTimeOffset.UtcNow.ToString());

        var s3Bucket = Bucket.FromBucketName(this, "bucket", $"{props.EnvironmentPrefix.ToLower()}sbmt-api-1");

        var policy = new Policy(this, "smbt-policy", new PolicyProps
        {
            PolicyName = "sbmt-policy",
            Roles = new Role[] { lambdaExecutionRole },
            Statements = new PolicyStatement[]
            {
                new PolicyStatement(new PolicyStatementProps
                {
                    Effect = Effect.ALLOW,
                    Actions = new string[] { "dynamodb:*" },
                    Resources = new string[]
                    {
                        table.TableArn,
                        table.TableArn + "/index/*",
                        table.TableStreamArn
                        //props.TestTable.TableArn,
                        //props.TestTable.TableArn + "/index/*"
                    }
                }),
                new PolicyStatement(new PolicyStatementProps
                {
                    Effect = Effect.ALLOW,
                    Actions = new string[] { "ses:SendEmail", "ses:VerifyEmailIdentity" },
                    Resources = new string[] { "*" }
                }),
                new PolicyStatement(new PolicyStatementProps
                {
                    Effect = Effect.ALLOW,
                    Actions = new string[] { "s3:*" },
                    Resources = new string[]
                    {
                        s3Bucket.BucketArn,
                        s3Bucket.BucketArn + "/*"
                    }
                })
            }
        });
    }
}
