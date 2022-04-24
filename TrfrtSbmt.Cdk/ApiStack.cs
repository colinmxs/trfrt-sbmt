﻿namespace TrfrtSbmt.Cdk;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
public class ApiStack : Stack
{
    public record ApiStackProps
    {
        public string Name { get; init; } = "TreefortSubmitApi";
    }
    
    public ApiStack(ApiStackProps props)
    {
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
            Code = new AssetCode("TrfrtSbmt.Api/publish.zip"),
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
            }
        });
        Amazon.CDK.Tags.Of(restApi).Add("Name", $"{props.Name}RestApi");
        Amazon.CDK.Tags.Of(lambdaExecutionRole).Add("Last Updated", DateTimeOffset.UtcNow.ToString());
    }
}
