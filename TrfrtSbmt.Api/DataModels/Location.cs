using Amazon.DynamoDBv2.Model;

namespace TrfrtSbmt.Api.DataModels;

public class Location : BaseEntity
{
    private const string PartitionKeyValuePrefix = "Location-";
    
    // name = Country|State|City
    public Location(string country, string state, string city) : base($"{PartitionKeyValuePrefix}{country}{state}{city}")
    public Location(Dictionary<string, AttributeValue> values) : base(values) {}
}