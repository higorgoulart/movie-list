using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace MovieList.WebAPI;

public class MvcApplication : HttpApplication
{
    protected void Application_Start()
    {
        AreaRegistration.RegisterAllAreas();
        
        Startup.ConfigureServices();
        
        GlobalConfiguration.Configure(Startup.Configure);
    }
}