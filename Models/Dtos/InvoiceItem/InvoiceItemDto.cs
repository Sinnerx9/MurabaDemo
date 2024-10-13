using MurabaDemo.Enums;
using MurabaDemo.Models.Dtos.Product;
using MurabaDemo.Models.Dtos.ProductDiscount;
using MurabaDemo.Models.Infrastructure;

namespace MurabaDemo.Models.Dtos.InvoiceItem;

public class InvoiceItemDto : FullAuditedEntitiyDto<Guid>
{
    public ProductDto product { get; set; }

    public int quantity { get; set; }
    public ProductDiscountDto? discount { get; set; }
    public decimal totalPrice => quantity * product.price;

    public decimal discountValue => (discount?.type ?? DiscountValueType.FIXED) == DiscountValueType.FIXED
        ? (discount?.discount ?? 0)
        : (((discount?.discount ?? 0) / 100) * totalPrice);

    public decimal totalNet => totalPrice - discountValue;
}