﻿using System;
using System.Net.Http;
using dotenv.net;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;
using MovieList.Application.Jobs;
using MovieList.Domain.Repositories;
using MovieList.Infrastructure;
using MovieList.Infrastructure.Repositories;
using Owin;

[assembly: OwinStartup(typeof(MovieList.WorkerService.Startup))]
namespace MovieList.WorkerService;

public class Startup
{
    public void Configuration(IAppBuilder app)
    {
        app.UseHangfireDashboard("/hangfire");
        app.UseHangfireServer();
    }
    
    public static void ConfigureServices()
    {
        var services = new ServiceCollection();
        
        DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));
        
        services.AddScoped(_ => new HttpClient
        {
            BaseAddress = new Uri("https://api.themoviedb.org/3/"),
            DefaultRequestHeaders =
            {
                { "accept", "application/json" },
                { "Authorization", $"Bearer {Environment.GetEnvironmentVariable("TMDB_ACCESS_TOKEN")}" },
            }
        });
        services.AddScoped(_ => new MovieListDbContext());
        services.AddScoped<IMovieRepository, MovieRepository>();
        services.AddScoped<TmdbJob>();

        var serviceProvider = services.BuildServiceProvider();
        
        GlobalConfiguration.Configuration
            .UseSqlServerStorage("Hangfire")
            .UseActivator(new HangfireJobActivator(serviceProvider));
    }

    private class HangfireJobActivator(IServiceProvider serviceProvider) : JobActivator
    {
        public override object ActivateJob(Type jobType)
        {
            return serviceProvider.GetService(jobType);
        }
    }
}