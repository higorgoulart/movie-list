using System;
using System.Linq;
using System.Threading.Tasks;
using MovieList.Domain.Entities;
using MovieList.Domain.Repositories;
using MovieList.Domain.Resources;
using MovieList.Domain.Services;

namespace MovieList.Application.Services;

public class MovieService(IMovieRepository repository) : IMovieService
{
    private const int PAGE_SIZE = 12;
    
    public async Task<PagedResponse<Movie>> GetAllAsync(int page, string titleFilter, int? yearFilter, double? minVoteFilter)
    {
        var totalRecords = await repository.GetCountAsync(
            titleFilter, 
            yearFilter, 
            minVoteFilter
        );

        var totalPages = (int)Math.Ceiling(totalRecords / (double)PAGE_SIZE);
        totalPages = Math.Max(totalPages, 1);

        var movies = await repository.GetAllAsync(
            skip: (page - 1) * PAGE_SIZE,
            take: PAGE_SIZE,
            titleFilter: titleFilter,
            yearFilter: yearFilter,
            minVoteFilter: minVoteFilter
        );

        return new PagedResponse<Movie>
        {
            TotalPages = totalPages,
            Results = movies.ToArray()
        };
    }
}