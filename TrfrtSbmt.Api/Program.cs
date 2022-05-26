using Amazon.DynamoDBv2;
using Microsoft.OpenApi.Models;
using TrfrtSbmt.Api.Features;
using TrfrtSbmt.Api.Features.Groupings;
using TrfrtSbmt.Api.Features.Submissions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();

// add typed appsettings file
var appSettings = new AppSettings(builder.Configuration);
builder.Services.AddSingleton(appSettings);

// Add other services to the container.
builder.Services.AddAWSService<IAmazonDynamoDB>();
builder.Services.AddMediatR(typeof(Program));

// Add AWS Lambda support.
builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);

// add swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opts => 
{
    opts.SwaggerDoc("v1", new OpenApiInfo()
    {
        Title = "Submit API",
        Version = "v1"
    });
    opts.OperationFilter<SwaggerCustomizations.CustomHeaderSwaggerAttribute>();
});


// configure auth
builder.Services.AddAuthorization()
    .ConfigureCognitoAuth(appSettings);

var app = builder.Build();
app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod())
    .UseHttpsRedirection()
    .UseAuthentication()
    .UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    var url = "swagger/v1/swagger.json";
    options.SwaggerEndpoint(url, "v1");
    options.RoutePrefix = string.Empty;
});

// endpoints
app.MapGet("/healthcheck", () => "Submit Api!");
app.MapPost("/submit", async (Submit.Command command, [FromServices] IMediator mediator) => await mediator.Send(command));
app.MapGet("/photo-upload-url", async (string fileName, string title, string description, string fileType, [FromServices] IMediator mediator) => await mediator.Send(new GetUploadUrl.Query(fileName, title, description, fileType)));
app.MapPost("/define-submission-grouping", async (DefineSubmissionGrouping.Command command, [FromServices] IMediator mediator) => await mediator.Send(command));

app.Run();
