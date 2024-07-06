using System.Reflection;
using Microsoft.EntityFrameworkCore;
using api_lambda_deployment.Abstractions;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace api_lambda_deployment;
/// <summary>
/// Extension Methods for the application
/// </summary>
public static class ExtensionMethods
{
    /// <summary>
    /// Applies the database migration
    /// </summary>
    /// <param name="app">The web application</param>
    /// <exception cref="InvalidOperationException">Thrown when the database migration fails</exception>
    public static void ApplyDbMigration(this WebApplication app)
    {
        using (var serviceScope = app.Services.CreateScope())
        {
            var db = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            if (db.Database.GetPendingMigrations().Any())
            {
                db.Database.Migrate();
            }
        }
    } 

    public static IServiceCollection AddEndPoints(this IServiceCollection services, Assembly assembly)
    {
        //using Assembly exclude Abstract class and Interfaces and take the IEndPoint Type
        //and register all as Transient Types
        ServiceDescriptor[] serviceDescriptors = assembly.DefinedTypes
                    .Where(type => type is { IsAbstract: false, IsInterface: false }&&
                    type.IsAssignableTo(typeof(IEndpoint)))
                    .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
                    .ToArray();
        services.TryAddEnumerable(serviceDescriptors);
                    
       return services;
    }

    public static IApplicationBuilder MapEndPoints(this WebApplication app, RouteGroupBuilder? routeGroupBuilder =null){
        IEnumerable<IEndpoint> endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();
        IEndpointRouteBuilder builder = routeGroupBuilder is null ? app: routeGroupBuilder;

        foreach (IEndpoint endpoint in endpoints)
        {
            endpoint.MapEndpoint(builder);
        }
        return app;    
    }



}