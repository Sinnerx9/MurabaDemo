using MurabaDemo.Models.Dtos.InvoiceItem;
using MurabaDemo.Models.Infrastructure;

namespace MurabaDemo.Models.Dtos.Invoice;

public class InvoiceDto : FullAuditedEntitiyDto<Guid>
{
    public string customerName { get; set; }

    public int invoiceNo { get; set; }

    public DateTime invoiceDate { get; set; }
    public decimal totalAmount { get; set; }
    public decimal totalDiscount { get; set; }
    public decimal netAmount { get; set; }
    public virtual ICollection<InvoiceItemDto> items { get; set; }   
}