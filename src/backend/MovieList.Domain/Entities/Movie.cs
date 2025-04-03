using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieList.Domain.Entities;

public class Movie
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    public string Title { get; set; }
    public string Overview { get; set; }
    public DateTime ReleaseDate { get; set; }
    public double Popularity { get; set; }
    public double VoteAverage { get; set; }
    public double VoteCount { get; set; }
    public string PosterPath { get; set; }
}