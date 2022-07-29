using Amazon.CDK.AWS.S3;

namespace TrfrtSbmt.Cdk;

public class S3Stack : Stack
{
    public Bucket Bucket { get; set; }
    public class S3StackProps : StackProps 
    {
        public string EnvironmentName { get; init; } = "Development";
        public string EnvironmentPrefix { get; init; } = "Development-";
    }
    public S3Stack(Construct scope, string id, S3StackProps props) : base(scope, id, props)
    {
        Amazon.CDK.Tags.Of(this).Add("Billing", "Treefort");
        Bucket = new Bucket(this, "Bucket", new BucketProps
        {
            BucketName = $"{props.EnvironmentPrefix.ToLower()}sbmt-api-1",
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
    }
}
