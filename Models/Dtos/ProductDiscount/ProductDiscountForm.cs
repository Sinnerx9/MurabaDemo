using MurabaDemo.Enums;

namespace MurabaDemo.Models.Dtos.ProductDiscount;

public class ProductDiscountForm
{
    public decimal discount { get; set; }
    public DiscountValueType type { get; set; }
    public string description { get; set; }
    public DateTime validFrom { get; set; }
    public DateTime validTo { get; set; }
}