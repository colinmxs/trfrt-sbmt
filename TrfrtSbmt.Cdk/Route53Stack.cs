using Amazon.CDK.AWS.Route53;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrfrtSbmt.Cdk;

internal class Route53Stack
{
    public Route53Stack(Construct scope, string id, StackProps props) : base(scope, id, props)
    {
        //var route53 = new RecordSet(this, "customdomain", new RecordSetProps
        //{
        //    RecordName = $"{subdomain}.{domain}",
        //    RecordType = RecordType.CNAME,
        //    Zone = HostedZone.FromLookup(this, "HostedZone", new HostedZoneProviderProps
        //    {
        //        DomainName = domain
        //    }),
        //    Target = RecordTarget.FromAlias(new ApiGateway(restApi))
        //});

        //Amazon.CDK.Tags.Of(route53).Add("Name", domain);
        //Amazon.CDK.Tags.Of(route53).Add("Last Updated", DateTimeOffset.UtcNow.ToString());
    }
}
