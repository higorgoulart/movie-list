using System.Data.Entity;
using MovieList.Domain.Entities;

namespace MovieList.Infrastructure;

public class MovieListDbContext() : DbContext("name=MovieListDb")
{
    public DbSet<Movie> Movies { get; set; }
}