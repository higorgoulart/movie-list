using System.Data.Entity;
using System.Diagnostics.CodeAnalysis;
using MovieList.Domain.Entities;

namespace MovieList.Infrastructure;

[ExcludeFromCodeCoverage]
public class MovieListDbContext() : DbContext("name=MovieListDb")
{
    public DbSet<Movie> Movies { get; set; }
}