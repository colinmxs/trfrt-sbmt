namespace TrfrtSbmt.Tests;

using Amazon.DynamoDBv2;
using CodenameGenerator;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TrfrtSbmt.Api;

internal static class TestFixture
{
    public static IServiceScopeFactory? ScopeFactory { get; }
    public static Generator NameGenerator { get; } = new Generator(" ", Casing.PascalCase, WordBank.Verbs, WordBank.Nouns );
    public static string Lorem = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
    public static Random Rand = new Random();


    static TestFixture()
    {
        var services = new ServiceCollection();
        // Add other services to the container.
        var appSettings = new AppSettings 
        {
            TableName = "Submissions-Tests"
        };
        services.AddSingleton(appSettings);
        services.AddAWSService<IAmazonDynamoDB>();
        services.AddSingleton(new ClaimsPrincipal(new List<ClaimsIdentity> { new ClaimsIdentity(new List<Claim> { new Claim("username", "colin") }) }));
        services.AddMediatR(typeof(Program));
        var provider = services.BuildServiceProvider();
        ScopeFactory = provider.GetService<IServiceScopeFactory>();
    }

    public static async Task ExecuteScopeAsync(Func<IServiceProvider, Task> action)
    {
        if (ScopeFactory == null)
        {
            throw new InvalidOperationException("ScopeFactory is null");
        }
        using (var scope = ScopeFactory.CreateScope())
        {
            var prov = scope.ServiceProvider;
            await action(prov);
        }
    }

    public static async Task<T> ExecuteScopeAsync<T>(Func<IServiceProvider, Task<T>> action)
    {
        if (ScopeFactory == null)
        {
            throw new InvalidOperationException("ScopeFactory is null");
        }
        using (var scope = ScopeFactory.CreateScope())
        {
            try
            {
                var result = await action(scope.ServiceProvider);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public static Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        return ExecuteScopeAsync(sp =>
        {
            var mediator = sp.GetService<IMediator>();
            if (mediator == null) throw new InvalidOperationException("mediator is null");
            return mediator.Send(request);
        });
    }

    public static Task SendAsync(IRequest request)
    {
        return ExecuteScopeAsync(sp =>
        {
            var mediator = sp.GetService<IMediator>();
            if (mediator == null) throw new InvalidOperationException("mediator is null");
            return mediator.Send(request);
        });
    }
}