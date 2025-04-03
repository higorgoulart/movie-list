using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using MovieList.Domain.Entities;
using MovieList.Domain.Repositories;

namespace MovieList.Infrastructure.Repositories;

[ExcludeFromCodeCoverage]
public class MovieRepository(MovieListDbContext context) : IMovieRepository
{
    public async Task<Movie> GetByIdAsync(int id)
    {
        return await context.Movies.FindAsync(id);
    }
    
    public async Task<IEnumerable<Movie>> GetAllAsync(
        int skip,
        int take, 
        string titleFilter, 
        int? yearFilter, 
        double? minVoteFilter)
    {
        var query = context.Movies.AsQueryable();

        if (!string.IsNullOrEmpty(titleFilter))
            query = query.Where(m => m.Title.Contains(titleFilter));

        if (yearFilter.HasValue)
            query = query.Where(m => m.ReleaseDate.Year == yearFilter);

        if (minVoteFilter.HasValue)
            query = query.Where(m => m.VoteAverage >= minVoteFilter);

        return await query
            .OrderByDescending(m => m.Popularity)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<int> GetCountAsync(
        string titleFilter, 
        int? yearFilter, 
        double? minVoteFilter)
    {
        var query = context.Movies.AsQueryable();

        if (!string.IsNullOrEmpty(titleFilter))
            query = query.Where(m => m.Title.Contains(titleFilter));

        if (yearFilter.HasValue)
            query = query.Where(m => m.ReleaseDate.Year == yearFilter);

        if (minVoteFilter.HasValue)
            query = query.Where(m => m.VoteAverage >= minVoteFilter);

        return await query.CountAsync();
    }

    public async Task AddOrUpdateAsync(Movie movie)
    {
        var existingMovie = await GetByIdAsync(movie.Id);
        if (existingMovie is null)
        {
            context.Movies.Add(movie);
        }
        else
        {
            existingMovie.Title = movie.Title;
            existingMovie.Overview = movie.Overview;
            existingMovie.ReleaseDate = movie.ReleaseDate;
            existingMovie.Popularity = movie.Popularity;
            existingMovie.VoteAverage = movie.VoteAverage;
            existingMovie.VoteCount = movie.VoteCount;
            existingMovie.PosterPath = movie.PosterPath;
        }
        
        await context.SaveChangesAsync();
    }
}