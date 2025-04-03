using System;
using Newtonsoft.Json;

namespace MovieList.Domain.Resources;

public class MovieResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Overview { get; set; }
    
    [JsonProperty("release_date")]
    public DateTime ReleaseDate { get; set; }
    public double Popularity { get; set; }
    
    [JsonProperty("vote_average")]
    public double VoteAverage { get; set; }
    
    [JsonProperty("vote_count")]
    public double VoteCount { get; set; }
    
    [JsonProperty("poster_path")]
    public string PosterPath { get; set; }
}