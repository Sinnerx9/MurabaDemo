using MurabaDemo.Models.Dtos.Invoice;
using MurabaDemo.Models.Dtos.Product;
using MurabaDemo.Models.Dtos.ProductDiscount;

namespace MurabaDemo.Models.Dtos.Reports;

public class SalesReport
{
    public ProductDto product { get; set; }
    public int totalQuantitySold { get; set; }
    public decimal totalAmount { get; set; }
    public decimal totalSalesAmount => totalAmount - totalDiscountAmount;
    public decimal totalDiscountAmount { get; set; }
}