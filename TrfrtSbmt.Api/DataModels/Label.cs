using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace TrfrtSbmt.Api.DataModels;

public class Label : BaseEntity
{
    protected override string SortKeyPrefix => $"{nameof(Label)}-";    
    public Label(string name, List<string> targets) : base(name, name) 
    {
        _attributes[nameof(Targets)] = new AttributeValue { SS = targets };
    }
    public Label(Dictionary<string, AttributeValue> values) : base(values) { }

    public List<string> Targets => _attributes[nameof(Targets)].SS;

    public override Task DeleteAsync(IAmazonDynamoDB db, string tableName)
    {
        throw new NotImplementedException();
    }
}