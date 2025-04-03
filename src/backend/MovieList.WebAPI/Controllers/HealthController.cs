using System.Web.Http;

namespace MovieList.WebAPI.Controllers;

public class HealthController : ApiController
{
    [HttpGet]
    [Route("api/health")]
    public IHttpActionResult HealthCheck() => Ok();
}