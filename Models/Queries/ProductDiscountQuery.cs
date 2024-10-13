using MurabaDemo.Enums;
using MurabaDemo.Helpers.Attributes;

namespace MurabaDemo.Models.Queries;

public class ProductDiscountQuery
{
    public Guid? productId { get; set; }
    public decimal? discount { get; set; }
    public DiscountValueType? type { get; set; }
    public string? description { get; set; }
    [MapToProperty("validFrom")]
    public DateTime? validFromStart { get; set; }
    [MapToProperty("validTo")]
    public DateTime? validToEnd { get; set; } 

}