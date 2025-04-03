using System;
using System.Net.Http;
using System.Threading.Tasks;
using MovieList.Domain.Entities;
using MovieList.Domain.Repositories;
using MovieList.Domain.Resources;
using Newtonsoft.Json;

namespace MovieList.Application.Jobs;

public class TmdbJob(IMovieRepository movieRepository)
{
    private readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri("https://api.themoviedb.org/3/"),
        DefaultRequestHeaders =
        {
            { "accept", "application/json" },
            { "Authorization", $"Bearer {Environment.GetEnvironmentVariable("TMDB_ACCESS_TOKEN")}" },
        }
    };

    public async Task Execute()
    {
        const int maxPages = 5;
        var page = 1;

        while (page <= maxPages)
        {
            var response = await _httpClient.GetAsync($"movie/popular?language=en-US&page={page}");
            if (!response.IsSuccessStatusCode)
                break;
            
            var body = await response.Content.ReadAsStringAsync();
            var pageResponse = JsonConvert.DeserializeObject<PagedResponse<MovieResponseDto>>(body);

            foreach (var movie in pageResponse.Results)
            {
                await movieRepository.AddOrUpdateAsync(new Movie
                {
                    Id = movie.Id,
                    Title = movie.Title,
                    Overview = movie.Overview,
                    ReleaseDate = movie.ReleaseDate,
                    Popularity = movie.Popularity,
                    VoteAverage = movie.VoteAverage,
                    VoteCount = movie.VoteCount,
                    PosterPath = movie.PosterPath
                });
            }

            page++;
        }
    }
}