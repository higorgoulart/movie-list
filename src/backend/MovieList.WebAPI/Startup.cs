using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Dependencies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;
using MovieList.Application.Services;
using MovieList.Domain.Repositories;
using MovieList.Domain.Services;
using MovieList.Infrastructure;
using MovieList.Infrastructure.Repositories;
using MovieList.WebAPI.Controllers;
using Newtonsoft.Json.Serialization;
using Owin;
using Swashbuckle.Application;

[assembly: OwinStartup(typeof(MovieList.WebAPI.Startup))]
namespace MovieList.WebAPI;

public class Startup
{
    public void Configuration(IAppBuilder app)
    {
    }
    
    public static void Configure(HttpConfiguration config)
    {
        var cors = new EnableCorsAttribute("*", "*", "*"); 
        
        config.EnableCors(cors);
        
        var jsonFormatter = config.Formatters.JsonFormatter;
        jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        
        config.Formatters.Remove(config.Formatters.XmlFormatter);
        
        config
            .EnableSwagger(c => c.SingleApiVersion("v1", "Movie List"))
            .EnableSwaggerUi();    
        
        config.MapHttpAttributeRoutes();
        config.Routes.MapHttpRoute(
            name: "API Default",
            routeTemplate: "api/{controller}/{id}",
            defaults: new { id = RouteParameter.Optional }
        );
    }

    public static void ConfigureServices()
    {
        var services = new ServiceCollection();
        
        services.AddScoped(_ => new MovieListDbContext());
        services.AddScoped<IMovieRepository, MovieRepository>();
        services.AddScoped<IMovieService, MovieService>();
        services.AddTransient<MovieController>();

        var serviceProvider = services.BuildServiceProvider();
        
        GlobalConfiguration.Configuration.DependencyResolver = new DefaultDependencyResolver(serviceProvider);
    }

    private class DefaultDependencyResolver(IServiceProvider serviceProvider) : IDependencyResolver
    {
        public IDependencyScope BeginScope()
        {
            return new DefaultDependencyResolver(serviceProvider);
        }

        public object GetService(Type serviceType)
        {
            return serviceProvider.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return serviceProvider.GetServices(serviceType);
        }

        public void Dispose()
        {
        }
    }
}