using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.S3;

namespace TrfrtSbmt.Cdk;

public class S3Stack : Stack
{
    public class S3StackProps : StackProps 
    {
        public string RoleArn { get; init; }
    }
    public S3Stack(Construct scope, string id, S3StackProps props) : base(scope, id, props)
    {
        Amazon.CDK.Tags.Of(this).Add("Billing", "Treefort");
        var bucket = new Bucket(this, "Bucket", new BucketProps
        {
            BucketName = "sbmt-api-1",
            Cors = new CorsRule[1] 
            {
                new CorsRule
                {
                    AllowedHeaders = new string[] { "*" },
                    AllowedMethods = new HttpMethods[] { HttpMethods.GET, HttpMethods.POST, HttpMethods.PUT, HttpMethods.DELETE, HttpMethods.HEAD },
                    AllowedOrigins = new string[] { "*" }
                }
            },
            PublicReadAccess = true
        });

        var bucketAccessPolicy = new Policy(this, "BucketPolicy", new PolicyProps
        {
            PolicyName = "sbmt-api-1-policy",
            Roles = new IRole[] { Role.FromRoleArn(this, "lambdaRole", props.RoleArn) },
            Statements = new PolicyStatement[]
                {
                    new PolicyStatement(new PolicyStatementProps
                    {
                        Effect = Effect.ALLOW,
                        Actions = new string[] { "s3:*" },
                        Resources = new string[]
                        {
                            bucket.BucketArn,
                            bucket.BucketArn + "/*"
                        }
                    })
                }
        });
        Amazon.CDK.Tags.Of(bucketAccessPolicy).Add("Name", "sbmt-api-1-policy");
    }
}
