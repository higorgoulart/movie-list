using System.Web.Mvc;
using Hangfire;
using MovieList.Application.Jobs;

namespace MovieList.WorkerService;

public class MvcApplication : System.Web.HttpApplication
{
    protected void Application_Start()
    {
        AreaRegistration.RegisterAllAreas();

        Startup.ConfigureServices();

        using (new BackgroundJobServer())
        {
            RecurringJob.AddOrUpdate<TmdbJob>(
                nameof(TmdbJob), 
                job => job.Execute(), 
                Cron.Hourly
            );
        }
    }
}