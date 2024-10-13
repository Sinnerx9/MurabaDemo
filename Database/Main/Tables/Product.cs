using System.ComponentModel.DataAnnotations.Schema;
using MurabaDemo.Database.Tables.Infrastructure;

namespace MurabaDemo.Database.Tables;

[Table("products")]
public class Product : FullAuditedEntitiy<Guid>
{
    public string name { get; set; }
    public string code { get; set; }
    public string partNo { get; set; }
    public decimal price { get; set; }

    public ICollection<ProductDiscount> discounts { get; set; }
}