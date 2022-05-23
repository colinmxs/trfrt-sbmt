using Amazon.DynamoDBv2.Model;

namespace TrfrtSbmt.Api.DataModels;

public class Submission : BaseEntity
{
    public Submission(Dictionary<string, AttributeValue> values) : base(values) { }

    public Submission() : base("","","")
    {
        throw new NotImplementedException();



            
            /* new Dictionary<string, AttributeValue>
            {
                ["PartitionKey"] = new AttributeValue { S = request.Name },
                ["SortKey"] = new AttributeValue { S = request.Fort },
                ["SubmissionGrouping"] = new AttributeValue { S = _settings.SubmissionGrouping },
                ["State"] = new AttributeValue { S = request.State },
                ["City"] = new AttributeValue { S = request.City },
                ["Country"] = new AttributeValue { S = request.Country },
                ["Description"] = new AttributeValue { S = request.Description },
                ["Image"] = new AttributeValue { S = request.Image },
                ["Website"] = new AttributeValue { S = request.Website },
                ["Genre"] = new AttributeValue { S = request.Genre },
                ["Links"] = new AttributeValue { S = JsonSerializer.Serialize(request.Links)},
                ["ContactInfo"] = new AttributeValue { S = JsonSerializer.Serialize(request.ContactInfo) }
            })); */

    }

}