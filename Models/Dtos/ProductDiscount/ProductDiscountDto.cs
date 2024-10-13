using System.Text.Json.Serialization;
using MurabaDemo.Enums;
using MurabaDemo.Models.Dtos.Product;
using MurabaDemo.Models.Infrastructure;

namespace MurabaDemo.Models.Dtos.ProductDiscount;

public class ProductDiscountDto : FullAuditedEntitiyDto<Guid>
{
    public Guid productId { get; set; }
    [JsonIgnore()]
    public ProductDto product { get; set; }
    public decimal discount { get; set; }
    public DiscountValueType type { get; set; }
    public string description { get; set; }
    public DateTime validFrom { get; set; }
    public DateTime validTo { get; set; }
}