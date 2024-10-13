using MurabaDemo.Database.Tables;
using MurabaDemo.Models.Dtos.Product;
using MurabaDemo.Models.Dtos.ProductDiscount;
using MurabaDemo.Models.Infrastructure;
using MurabaDemo.Models.Queries;

namespace MurabaDemo.Interfaces;

public interface  IProductService
{
    public Task<Response<ProductDto>> InsertAsync(ProductForm entity, bool transaction = false,
        CancellationToken ct = default);

    public Task<Response<ProductDto>> UpdateAsync(Guid id, ProductForm entity, bool transaction = false,
        CancellationToken ct = default);

    public Task<PaginatedResponse<ProductDto>> GetAsync(ProductQuery query, PaginationQuery pagination,
        CancellationToken ct = default);

    public Task<Response<ProductDto>> ShowAsync(Guid id, CancellationToken ct = default);

    public Task<Response<ProductDto>> DeleteAsync(Guid id, CancellationToken ct = default);
    
    
    
    public Task<Response<ProductDiscountDto>> InsertDiscountAsync(Guid productId, ProductDiscountForm entity, bool transaction = false,
        CancellationToken ct = default);

    public Task<PaginatedResponse<ProductDiscountDto>> GetAllDiscountsAsync(ProductDiscountQuery query, PaginationQuery pagination, CancellationToken ct = default);

    public Task<Response<ProductDiscountDto>> DeleteDiscountsAsync(Guid id, CancellationToken ct = default);
}