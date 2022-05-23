using Amazon.DynamoDBv2.Model;

namespace TrfrtSbmt.Api.DataModels;

public class Rating : BaseEntity
{
    public Rating(Dictionary<string, AttributeValue> values) : base(values)
    {
    }
}