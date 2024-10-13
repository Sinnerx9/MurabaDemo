using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MurabaDemo.Database.Tables.Infrastructure;

namespace MurabaDemo.Database.Tables;

[Table("invoices")]
public class Invoice : FullAuditedEntitiy<Guid>
{
    public string customerName { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int invoiceNo { get; set; }

    public DateTime invoiceDate { get; set; }

    public virtual ICollection<InvoiceItem> items { get; set; }
}