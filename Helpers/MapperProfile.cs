using AutoMapper;
using MurabaDemo.Database.Tables;
using MurabaDemo.Models.Dtos.Invoice;
using MurabaDemo.Models.Dtos.InvoiceItem;
using MurabaDemo.Models.Dtos.Product;
using MurabaDemo.Models.Dtos.ProductDiscount;
using MurabaDemo.Models.Dtos.User;

namespace MurabaDemo.Helpers;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<User, UserDto>();
        
        CreateMap<Product, ProductDto>();
        CreateMap<ProductDiscount,ProductDiscountDto>();
        
        CreateMap<InvoiceForm, Invoice>();
        CreateMap<Invoice, InvoiceDto>().AfterMap(((invoice, dto) =>
        {
            dto.totalAmount = dto.items.Select(x => x.totalPrice).Sum();
            dto.totalDiscount = dto.items.Select(x => x.discountValue).Sum();
            dto.netAmount = dto.items.Select(x => x.totalNet).Sum();
        }));
        CreateMap<InvoiceItem, InvoiceItemDto>();
        CreateMap<InvoiceItemForm, InvoiceItem>();
    }
}