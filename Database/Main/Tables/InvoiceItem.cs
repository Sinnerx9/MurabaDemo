using System.ComponentModel.DataAnnotations.Schema;
using MurabaDemo.Database.Tables.Infrastructure;

namespace MurabaDemo.Database.Tables;

[Table("invoiceItems")]
public class InvoiceItem :  FullAuditedEntitiy<Guid>
{
    [ForeignKey(nameof(product))]
    public Guid productId { get; set; }
    public virtual Product product { get; set; }
    
    public int quantity { get; set; }
    
    [ForeignKey(nameof(discount))]
    public Guid? discountId { get; set; }
    public virtual ProductDiscount? discount { get; set; }
}