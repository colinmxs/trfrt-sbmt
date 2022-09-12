using Amazon.DynamoDBv2.Model;

namespace TrfrtSbmt.Api.DataModels
{
    public class Vote : BaseEntity
    {
        public Vote(Dictionary<string, AttributeValue> values) : base(values) { }

        public Vote(string submissionId, int value, string voter) : base(submissionId, voter, $"{nameof(Vote)}-{voter}", $"{nameof(Vote)}-{voter}", voter) 
        {
            _attributes[nameof(Value)] = new AttributeValue { S = value.ToString() };
        }
        
        protected override string SortKeyPrefix => $"{nameof(Vote)}-";

        public int Value => int.Parse(_attributes[nameof(Value)].S);
    }
}
