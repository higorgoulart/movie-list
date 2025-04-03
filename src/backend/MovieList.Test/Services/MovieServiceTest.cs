using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using MovieList.Application.Services;
using MovieList.Domain.Entities;
using MovieList.Domain.Repositories;
using Xunit;

namespace MovieList.Test.Services;

public class MovieServiceTests
{
    private readonly Mock<IMovieRepository> _repositoryMock;
    private readonly MovieService _movieService;

    public MovieServiceTests()
    {
        _repositoryMock = new Mock<IMovieRepository>();
        _movieService = new MovieService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnPagedResponse()
    {
        var movies = new List<Movie>
        {
            new() { Id = 1, Title = "Movie 1", ReleaseDate = new DateTime(2020, 1, 1), VoteAverage = 8.0 },
            new() { Id = 2, Title = "Movie 2", ReleaseDate = new DateTime(2021, 1, 1), VoteAverage = 7.5 }
        };

        _repositoryMock
            .Setup(r => r.GetCountAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<double?>()))
            .ReturnsAsync(15);

        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<double?>()))
            .ReturnsAsync(movies);

        var result = await _movieService.GetAllAsync(1, "", null, null);

        Assert.Equal(2, result.Results.Length);
        Assert.Equal(2, result.TotalPages);
        Assert.Contains(result.Results, m => m.Id == 1);
        Assert.Contains(result.Results, m => m.Title == "Movie 1");
        Assert.Contains(result.Results, m => m.Overview == null);
        Assert.Contains(result.Results, m => m.ReleaseDate.Year == 2020);
        Assert.Contains(result.Results, m => m.VoteAverage == 8);
        Assert.Contains(result.Results, m => m.VoteCount == 0);
        Assert.Contains(result.Results, m => m.PosterPath == null);
        Assert.Contains(result.Results, m => m.Popularity == 0);
    }

    [Fact]
    public async Task GetAllAsync_ShouldHandleZeroMovies()
    {
        _repositoryMock
            .Setup(r => r.GetCountAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<double?>()))
            .ReturnsAsync(0);

        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<double?>()))
            .ReturnsAsync(new List<Movie>());

        var result = await _movieService.GetAllAsync(1, "", null, null);

        Assert.Empty(result.Results);
        Assert.Equal(1, result.TotalPages);
    }

    [Fact]
    public async Task GetAllAsync_ShouldCalculateTotalPagesCorrectly()
    {
        _repositoryMock
            .Setup(r => r.GetCountAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<double?>()))
            .ReturnsAsync(25);

        var result = await _movieService.GetAllAsync(1, "", null, null);

        Assert.Equal(3, result.TotalPages);
    }
}