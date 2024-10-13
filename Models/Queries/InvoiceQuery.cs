using MurabaDemo.Helpers.Attributes;

namespace MurabaDemo.Models.Queries;

public class InvoiceQuery
{
    public string customerName { get; set; }

    public int invoiceNo { get; set; }

    [MapToProperty("invoiceDate")]
    public DateTime invoiceDateStart { get; set; }
    
    [MapToProperty("invoiceDate")]
    public DateTime invoiceDateEnd { get; set; }
}