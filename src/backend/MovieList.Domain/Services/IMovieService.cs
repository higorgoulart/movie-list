using System.Threading.Tasks;
using MovieList.Domain.Entities;
using MovieList.Domain.Resources;

namespace MovieList.Domain.Services;

public interface IMovieService
{
    Task<PagedResponse<Movie>> GetAllAsync(int page, string titleFilter, int? yearFilter, double? minVoteFilter);
}