using System.ComponentModel.DataAnnotations.Schema;
using MurabaDemo.Database.Tables.Infrastructure;
using MurabaDemo.Enums;

namespace MurabaDemo.Database.Tables;


[Table("productDiscounts")]
public class ProductDiscount : FullAuditedEntitiy<Guid>
{
    [ForeignKey(nameof(product))]
    public Guid productId { get; set; }
    public Product product { get; set; }
    public decimal discount { get; set; }
    public DiscountValueType type { get; set; }
    public string description { get; set; }
    public DateTime validFrom { get; set; }
    public DateTime validTo { get; set; }
}