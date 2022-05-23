namespace TrfrtSbmt.Api.Features.Submissions;

public class ListSubmissionsByGrouping
{
    public record Query(string GroupingName) : IRequest<Result>;    

    public record Result;

    public class QueryHandler : IRequestHandler<Query, Result>
    {
        public Task<Result> Handle(Query request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}