using Amazon.DynamoDBv2.Model;

namespace TrfrtSbmt.Api.DataModels;

public partial class Festival
{
    public class Fort : BaseEntity
    {
        protected override string SortKeyPrefix => $"{nameof(Fort)}-";
        
        public Fort(Dictionary<string, AttributeValue> dictionary) : base(dictionary) { }
        public Fort(string name, string groupingId) : base(groupingId, name, name) { }
    }
}