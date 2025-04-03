
using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

namespace MovieList.Infrastructure.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<MovieList.Infrastructure.MovieListDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }
    } 
}