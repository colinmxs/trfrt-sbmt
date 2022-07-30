namespace TrfrtSbmt.Cdk.Stacks;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Route53;
using Amazon.CDK.AWS.Route53.Targets;

public class ApiStack : Stack
{
    public class ApiStackProps : StackProps
    {
        public string EnvironmentName { get; init; } = "Development";
        public string EnvironmentPrefix { get; init; } = "Development-";
        public string Name { get; init; } = "TreefortSubmitApi";
        public string Region { get; init; } = "us-east-1";
        public string CertId { get; init; } = string.Empty;
        //public string[] OtherRegions { get; init; } = new string[] { "us-west-2" };
    }

    public Role LambdaExecutionRole { get; init; }

    public ApiStack(Construct scope, string id, ApiStackProps props) : base(scope, id, props)
    {
        Amazon.CDK.Tags.Of(this).Add("Billing", "Treefort");
        var accountId = (string)scope.Node.TryGetContext("accountid");
        var domain = (string)scope.Node.TryGetContext("domain");
        var subdomain = (string)scope.Node.TryGetContext("subdomain");

        LambdaExecutionRole = new Role(this, "ApiLambdaExecutionRole", new RoleProps
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
        Amazon.CDK.Tags.Of(LambdaExecutionRole).Add("Name", $"{props.EnvironmentPrefix}{props.Name}LambdaExecutionRole");
        Amazon.CDK.Tags.Of(LambdaExecutionRole).Add("Last Updated", DateTimeOffset.UtcNow.ToString());

        var lambdaFunction = new Function(this, "LambdaFunction", new FunctionProps
        {
            Code = new AssetCode($"{Utilities.GetDirectory("TrfrtSbmt.Api")}/publish.zip"),
            Handler = "TrfrtSbmt.Api",
            Runtime = Runtime.DOTNET_6,
            Timeout = Duration.Seconds(10),
            FunctionName = $"{props.EnvironmentPrefix}{props.Name}LambdaFunction",
            MemorySize = 2048,
            RetryAttempts = 1,
            Role = LambdaExecutionRole,
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
    }
}
