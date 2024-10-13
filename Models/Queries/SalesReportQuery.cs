using MurabaDemo.Helpers.Attributes;

namespace MurabaDemo.Models.Queries;

public class SalesReportQuery
{
    public Guid? productId { get; set; }
    [MapToProperty("createdAt")] public DateTime? fromStart { get; set; }
    [MapToProperty("createdAt")] public DateTime? toEnd { get; set; }
}