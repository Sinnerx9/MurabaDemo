namespace MurabaDemo.Models.Infrastructure;

public class PaginatedResponse<T> : Response<List<T>>
{
    public int page { get; set; }
    public int pageSize { get; set; }
    public int total { get; set; }
    public int totalPages => (int)Math.Ceiling(total / (float)pageSize);

}