using Amazon.DynamoDBv2.Model;

namespace TrfrtSbmt.Api.DataModels;

public class Fort : BaseEntity
{
    protected override string SortKeyPrefix => $"{nameof(Fort)}-";
        
    public Fort(Dictionary<string, AttributeValue> dictionary) : base(dictionary) { }
    public Fort(string festivalId, string name, string createdBy) : base(festivalId, name, name, createdBy) { }
    
}
