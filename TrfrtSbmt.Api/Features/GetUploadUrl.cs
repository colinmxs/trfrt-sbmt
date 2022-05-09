using Amazon.DynamoDBv2;
using Amazon.S3;
using Amazon.S3.Model;

namespace TrfrtSbmt.Api.Features;

public class GetUploadUrl
{
    public record Query(string FileName, string Title, string Description, string FileType) : IRequest<Result>;

    public record Result(string UploadUrl);

    public class QueryHandler : IRequestHandler<Query, Result>
    {
        private readonly IAmazonS3 _s3;
        private readonly AppSettings _settings;

        public QueryHandler(IAmazonS3 s3, AppSettings appSettings)
        {            
            _s3 = s3;
            _settings = appSettings;           
        }

        public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
        {
            var objectKey = $"{request.FileName}";
            var metadataHeaderPrefix = "x-amz-meta-";

            var getPreSignedUrlRequest = new GetPreSignedUrlRequest
            {
                BucketName = _settings.BucketName,
                Key = objectKey,
                Verb = HttpVerb.PUT,
                Expires = DateTime.Now.AddMinutes(5)
            };

            getPreSignedUrlRequest.Parameters[metadataHeaderPrefix + "title"] = request.Title;
            getPreSignedUrlRequest.Parameters[metadataHeaderPrefix + "description"] = request.Description;
            getPreSignedUrlRequest.Parameters[metadataHeaderPrefix + "filetype"] = request.FileType;
            string url = _s3.GetPreSignedURL(getPreSignedUrlRequest);

            return new Result(url);
        }
    }
}