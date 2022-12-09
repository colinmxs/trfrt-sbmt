using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Text;
using Amazon.Runtime.Internal.Util;
using static TrfrtSbmt.Api.Features.Submissions.GetUploadUrl;
using TrfrtSbmt.Domain;
using TrfrtSbmt.Api.Features.Festivals;

namespace TrfrtSbmt.Api.Features.Search;

public class Search
{
    public record SearchQuery(string SearchTerm, int PageSize = 20, int Page = 1) : IRequest<SearchResult>;

    public record SearchResult
    {
        public int TotalPages { get; internal set; }
        public int Page { get; internal set; }
        public List<ViewModel> Results { get; internal set; }
        public int PageSize { get; internal set; }
    }

    public record ViewModel
    {
        private BaseEntity ri;

        public ViewModel(BaseEntity ri)
        {
            this.ri = ri;
        }

        public string Name => ri.Name;
        //public string 
    }

    public record SearchHandler : IRequestHandler<SearchQuery, SearchResult>
    {
        private readonly IAmazonDynamoDB _db;
        private readonly AppSettings _settings;
        private readonly ClaimsPrincipal _user;
        private readonly ResultCache _cache;

        public SearchHandler(IAmazonDynamoDB db, AppSettings settings, ClaimsPrincipal user, ResultCache cache)
        {
            _db = db;
            _settings = settings;
            _user = user;
            _cache = cache;
        }
        public async Task<SearchResult> Handle(SearchQuery request, CancellationToken cancellationToken)
        {
            var result = new SearchResult();

            string statement = BuildQuery(request.SearchTerm);

            try
            {
                var cached = _cache.Get(request.SearchTerm);
                if (cached == null)
                {
                    cached = await RunQuery(new List<BaseEntity>(), statement);
                    _cache.Store(request.SearchTerm, cached, DateTime.UtcNow.AddMinutes(15) - DateTime.UtcNow);
                }

                result.Results.AddRange(cached.Select(ri =>
                {
                    return new ViewModel(ri);
                }));
            }
            catch (AmazonDynamoDBException)
            {
                Console.WriteLine(statement);
                throw;
            }






            result.TotalPages = (int)Math.Ceiling((double)result.Results.Count / request.PageSize);
            //result.Results = result.Results.OrderBy(x => x.LastName).ThenBy(x => x.FirstName).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();
            result.Page = request.Page;
            result.PageSize = request.PageSize;




            return result;
        }

        private string BuildQuery(string searchString)
        {
            var alphaNumRegex = new Regex("[^a-zA-Z0-9]");
            var splitSearch = searchString.Split(' ').Select(split => alphaNumRegex.Replace(split, "")).ToArray();
            var stringBuilder = new StringBuilder()
                .Append($"SELECT * FROM \"{_settings.TableName}\" WHERE contains(\"SearchString\", '{splitSearch[0].ToUpperInvariant()}')");

            for (int i = 1; i < splitSearch.Length; i++)
            {
                stringBuilder.Append($" AND contains(\"SearchString\", '{splitSearch[i].ToUpperInvariant()}')");
            }
            var statement = stringBuilder.ToString();
            return statement;
        }

        private async Task<List<BaseEntity>> RunQuery(List<BaseEntity> list, string statement, string? pageToken = null)
        {
            var response = await _db.ExecuteStatementAsync(new ExecuteStatementRequest
            {
                NextToken = pageToken,
                Statement = statement,
                
            });

            if (response.Items != null)
            {
                foreach (var item in response.Items)
                {
                    
                }
                list.AddRange(response.Items.Select(ri => new SearchResultEntity(ri)));
            }

            if (response.NextToken != null)
            {
                await RunQuery(list, statement, response.NextToken);
            }

            return list;
        }
        
        private class SearchResultEntity : BaseEntity
        {
            public SearchResultEntity(Dictionary<string, AttributeValue> values) : base(values)
            {
            }

            public override string SortKeyPrefix => throw new NotImplementedException();
            
        }
    }
}
