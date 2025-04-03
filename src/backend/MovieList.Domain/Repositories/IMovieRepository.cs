using System.Collections.Generic;
using System.Threading.Tasks;
using MovieList.Domain.Entities;

namespace MovieList.Domain.Repositories;

public interface IMovieRepository
{
    Task<Movie> GetByIdAsync(int id);
    Task<IEnumerable<Movie>> GetAllAsync(int skip, int take, string titleFilter, int? yearFilter, double? minVoteFilter);
    Task<int> GetCountAsync(string titleFilter, int? yearFilter, double? minVoteFilter);
    Task AddOrUpdateAsync(Movie movie);
}