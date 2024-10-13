using System.Text.Json.Serialization;
using MurabaDemo.Database.Tables.Infrastructure;
using MurabaDemo.Models.Dtos.ProductDiscount;
using MurabaDemo.Models.Infrastructure;

namespace MurabaDemo.Models.Dtos.Product;

public class ProductDto  : FullAuditedEntitiyDto< Guid>
{
    public string name { get; set; }
    public string code { get; set; }
    public string partNo { get; set; }
    public int totalDiscounts => discounts.Count;
    public decimal price { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public virtual ICollection<ProductDiscountDto> discounts { get; set; }
}