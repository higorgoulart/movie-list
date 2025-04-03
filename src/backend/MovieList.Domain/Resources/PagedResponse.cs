namespace MovieList.Domain.Resources;

public class PagedResponse<T>
{
    public int TotalPages { get; set; }

    public T[] Results { get; set; }
}