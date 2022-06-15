using System.Runtime.CompilerServices;
using System.Web;
using Amazon.DynamoDBv2;
using Amazon.S3;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using TrfrtSbmt.Api.Features.Festivals;
using TrfrtSbmt.Api.Features.Forts;
using TrfrtSbmt.Api.Features.Labels;
using TrfrtSbmt.Api.Features.Submissions;

[assembly: InternalsVisibleTo("TrfrtSbmt.Tests")]

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();

// add typed appsettings file
var appSettings = new AppSettings(builder.Configuration);
builder.Services.AddSingleton(appSettings);

// Add other services to the container.
builder.Services.AddAWSService<IAmazonDynamoDB>();
builder.Services.AddAWSService<IAmazonS3>();
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
    opts.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    opts.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });

    //opts.OperationFilter<SwaggerCustomizations.CustomHeaderSwaggerAttribute>();
});


// configure auth
builder.Services.AddAuthorization(opts =>
{
    // add auth policy called admin
    opts.AddPolicy("admin", policy => policy.RequireClaim("cognito:groups", "admin"));
})
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

// health check
app.MapGet("/healthcheck", () => "Submit Api!")
    .RequireAuthorization();

// festivals
app.MapGet("/festivals", async (bool activeOnly, bool? submissionsOpen, int pageSize, string? paginationKey, [FromServices] IMediator mediator) 
    => await mediator.Send(new ListFestivalsQuery(activeOnly, submissionsOpen ?? false, pageSize, paginationKey)))
    .RequireAuthorization();

app.MapPost("/festivals", async (AddFestivalCommand command, [FromServices] IMediator mediator)
    => await mediator.Send(command))
    .RequireAuthorization("admin");
    

app.MapDelete("/festivals/{festivalId}", async (string festivalId, [FromServices] IMediator mediator)
    => await mediator.Send(new DeleteFestivalCommand(festivalId)))
    .RequireAuthorization("admin");

// forts
app.MapGet("/festivals/{festivalId}/forts", async (string festivalId, int pageSize, string ? paginationKey,[FromServices] IMediator mediator)
    => await mediator.Send(new ListForts.ListFortsQuery(festivalId, pageSize, paginationKey)))
    .RequireAuthorization();

app.MapPost("/festivals/{festivalId}/forts", async (string festivalId, AddFort.AddFortCommand command, [FromServices] IMediator mediator) =>    
{
    command.FestivalId = festivalId;
    return await mediator.Send(command);
})
    .RequireAuthorization("admin");

app.MapDelete("/festivals/{festivalId}/forts/{fortId}", async (string festivalId, string fortId, [FromServices] IMediator mediator)
    => await mediator.Send(new DeleteFort.DeleteFortCommand(fortId)))
    .RequireAuthorization("admin");

// submissions
app.MapPost("/festivals/{festivalId}/forts/{fortId}/submissions", async (string festivalId, string fortId, AddSubmission.AddSubmissionCommand command, [FromServices] IMediator mediator) => 
{
    command.FestivalId = festivalId;
    command.FortId = fortId;
    return await mediator.Send(command); 
})
    .RequireAuthorization();

app.MapGet("/festivals/{festivalId}/forts/{fortId}/submissions", async (string festivalId, string fortId, int pageSize, string? paginationKey, [FromServices] IMediator mediator)
    => await mediator.Send(new ListSubmissions.ListSubmissionsQuery(festivalId, fortId, pageSize, paginationKey)))
    .RequireAuthorization();

app.MapGet("/festivals/{festivalId}/forts/{fortId}/submissions/{submissionId}", async (string festivalId, string fortId, string submissionId, [FromServices] IMediator mediator)
    => await mediator.Send(new GetSubmission.GetSubmissionQuery(festivalId, fortId, submissionId)))
    .RequireAuthorization();

//app.MapGet("/photo-upload-url", async (string fileName, string title, string description, string fileType, [FromServices] IMediator mediator) => await mediator.Send(new GetUploadUrl.Query(fileName, title, description, fileType)));

app.Run();
