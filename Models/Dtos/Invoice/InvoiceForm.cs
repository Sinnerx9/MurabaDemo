using MurabaDemo.Models.Dtos.InvoiceItem;

namespace MurabaDemo.Models.Dtos.Invoice;

public class InvoiceForm
{
    public string customerName { get; set; }
    public ICollection<InvoiceItemForm> items { get; set; }
}