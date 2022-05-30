namespace TrfrtSbmt.Api.Features.Submissions;

public class ListSubmissions
{
    public record ListSubmissionQuery(string FortId) : IRequest<Result>;    

    public record Result;

    public class QueryHandler : IRequestHandler<ListSubmissionQuery, Result>
    {
        public Task<Result> Handle(ListSubmissionQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}