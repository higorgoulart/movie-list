using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using MovieList.Application.Jobs;
using MovieList.Domain.Entities;
using MovieList.Domain.Repositories;
using MovieList.Domain.Resources;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using Xunit;

namespace MovieList.Test.Jobs;

public class TmdbJobTests
{
    private readonly Mock<IMovieRepository> _movieRepositoryMock;
    private readonly MockHttpMessageHandler _mockHttp;
    private readonly TmdbJob _tmdbJob;

    public TmdbJobTests()
    {
        _movieRepositoryMock = new Mock<IMovieRepository>();
        _mockHttp = new MockHttpMessageHandler();
        _tmdbJob = new TmdbJob(_movieRepositoryMock.Object, new HttpClient(_mockHttp)
        {
            BaseAddress = new Uri("https://localhost/")
        });
    }

    [Fact]
    public async Task Execute_ShouldFetchAndSaveMovies()
    {
        const int pages = 5;
        var fakeResponse = new PagedResponse<MovieResponseDto>
        {
            Results =
            [
                new MovieResponseDto
                {
                    Id = 1, Title = "Fake Movie", Overview = "Description",
                    ReleaseDate = DateTime.Now, Popularity = 10, VoteAverage = 8, VoteCount = 100, PosterPath = "/path.jpg"
                }
            ]
        };
        
        for (var i = 1; i <= pages; i++)
            _mockHttp
                .When($"https://localhost/movie/popular?language=en-US&page={i}")
                .Respond("application/json", JsonConvert.SerializeObject(fakeResponse));
        
        await _tmdbJob.Execute();
        
        _movieRepositoryMock.Verify(m => m.AddOrUpdateAsync(It.IsAny<Movie>()), Times.Exactly(5));
    }

    [Fact]
    public async Task Execute_ShouldStopOnFailedResponse()
    {
        _mockHttp
            .When("https://localhost/movie/popular?language=en-US&page=1")
            .Respond(HttpStatusCode.BadRequest);
        
        await _tmdbJob.Execute();
        
        _movieRepositoryMock.Verify(m => m.AddOrUpdateAsync(It.IsAny<Movie>()), Times.Never);
    }
}
