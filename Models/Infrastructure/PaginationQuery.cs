namespace MurabaDemo.Models.Infrastructure;


public class PaginationQuery
{
    public int page { get; set; } = 1;
    public int pageSize { get; set; } = 10;
}