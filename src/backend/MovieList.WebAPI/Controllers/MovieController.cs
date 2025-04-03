using System.Threading.Tasks;
using System.Web.Http;
using MovieList.Domain.Services;

namespace MovieList.WebAPI.Controllers;

public class MovieController(IMovieService movieService) : ApiController
{
    [HttpGet]
    [Route("api/movies")]
    public async Task<IHttpActionResult> GetMovies(
        [FromUri] int page = 1,
        [FromUri] string title = null,
        [FromUri] int? year = null,
        [FromUri] double? minVote = null)
    {
        var result = await movieService.GetAllAsync(
            page,
            title,
            year,
            minVote
        );
    
        return Ok(result);
    }
}