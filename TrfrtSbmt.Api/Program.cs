var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();
// add typed appsettings file
ConfigurationManager configuration = builder.Configuration;
var appSettings = new AppSettings(configuration);
builder.Services.AddSingleton(appSettings);

// Add other services to the container.
builder.Services.AddMediatR(typeof(Program));

// Add AWS Lambda support.
builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);

// configure auth
builder.Services.AddAuthorization();

var app = builder.Build();
app.UseCors(builder => builder
              .AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// endpoints
app.MapGet("/", () => "Treefort Submit Api!");

app.Run();
