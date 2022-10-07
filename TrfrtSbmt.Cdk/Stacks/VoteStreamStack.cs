using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.Events.Targets;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrfrtSbmt.Cdk.Stacks;

public class VoteStreamStack : Stack
{
    public class VoteStreamStackProps : StackProps
    {
        public string EnvironmentName { get; init; } = "Development";
        public string EnvironmentPrefix { get; init; } = "Development-";
        public string Name { get; init; } = "TreefortSubmitVoteProcessor";
    }
    public Role LambdaExecutionRole { get; init; }
    public VoteStreamStack(Construct scope, string id, VoteStreamStackProps props) : base(scope, id, props)
    {
        Amazon.CDK.Tags.Of(this).Add("Billing", "Treefort");

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

        var targetFunction = new Function(this, "VoteStream.Function", new FunctionProps 
        {
            Runtime = Runtime.DOTNET_6,
            Code = new AssetCode($"{Utilities.GetDirectory("TrfrtSbmt.VoteStreamProcessor")}/publish.zip"),
            Handler = "TrfrtSbmt.VoteStreamProcessor",
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
        Amazon.CDK.Tags.Of(targetFunction).Add("Name", $"{props.EnvironmentPrefix}{props.Name}LambdaFunction");
        Amazon.CDK.Tags.Of(targetFunction).Add("Last Updated", DateTimeOffset.UtcNow.ToString());        
    }
}
