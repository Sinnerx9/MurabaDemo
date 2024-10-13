using AutoMapper;
using MurabaDemo.Database.Tables;
using MurabaDemo.Helpers.Extensions;
using MurabaDemo.Interfaces;
using MurabaDemo.Models.Dtos.Product;
using MurabaDemo.Models.Dtos.ProductDiscount;
using MurabaDemo.Models.Infrastructure;
using MurabaDemo.Models.Queries;

namespace MurabaDemo.Services;

public class ProductService(IBaseService _base, IMapper mapper) : IProductService

{
    public async Task<Response<ProductDto>> InsertAsync(ProductForm entity, bool transaction = false,
        CancellationToken ct = default)
    {
        Product product = entity.MapTo<Product>();
        try
        {
            var result = await _base.InsertAsync(product, ct: ct);
            return new Response<ProductDto>()
            {
                result = mapper.Map<ProductDto>(result),
                statusCode = 200,
                message = "OK"
            };
        }
        catch (Exception ex)
        {
            return new Response<ProductDto>()
            {
                errors = [ex.Message],
                statusCode = ex.Message.Contains("not found") ? 404 : 400,
                message = "Failed"
            };
        }
    }

    public async Task<Response<ProductDto>> UpdateAsync(Guid id, ProductForm entity, bool transaction = false,
        CancellationToken ct = default)
    {
        try
        {
            var result = await _base.UpdateAsync<Guid, Product, ProductForm>(id, entity, ct: ct);
            return new Response<ProductDto>()
            {
                result = mapper.Map<ProductDto>(result),
                statusCode = 200,
                message = "OK"
            };
        }
        catch (Exception ex)
        {
            return new Response<ProductDto>()
            {
                errors = [ex.Message],
                statusCode = ex.Message.Contains("not found") ? 404 : 400,
                message = "Failed"
            };
        }
    }

    public async Task<PaginatedResponse<ProductDto>> GetAsync(ProductQuery query, PaginationQuery pagination,
        CancellationToken ct = default)
    {
        var (result, total) = await _base.GetAsync<ProductQuery, Product>(query, pagination, ct);
        return new PaginatedResponse<ProductDto>()
        {
            result = mapper.Map<List<ProductDto>>(result),
            statusCode = 200,
            page = pagination.page,
            pageSize = pagination.pageSize,
            total = total,
            message = "OK",
        };
    }

    public async Task<Response<ProductDto>> ShowAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var result = await _base.ShowAsync<Guid, Product>(id, ct);
            return new Response<ProductDto>()
            {
                result = result == null ? null : mapper.Map<ProductDto>(result),
                statusCode = result == null ? 404 : 200,
                message = result == null ? "Product not found" : "OK"
            };
        }
        catch (Exception ex)
        {
            return new Response<ProductDto>()
            {
                errors = [ex.Message],
                statusCode = ex.Message.Contains("not found") ? 404 : 400,
                message = "Failed"
            };
        }
    }

    public async Task<Response<ProductDto>> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var result = await _base.DeleteAsync<Guid, Product>(id, ct);
            return new Response<ProductDto>()
            {
                result = mapper.Map<ProductDto>(result),
                statusCode = 200,
                message = "OK"
            };
        }
        catch (Exception ex)
        {
            return new Response<ProductDto>()
            {
                errors = [ex.Message],
                statusCode = ex.Message.Contains("not found") ? 404 : 400,
                message = "Failed"
            };
        }
    }

    public async Task<Response<ProductDiscountDto>> InsertDiscountAsync(Guid productId, ProductDiscountForm entity,
        bool transaction = false,
        CancellationToken ct = default)
    {
        var product = await _base.ShowAsync<Guid, Product>(productId);
        if (product == null)
            return new Response<ProductDiscountDto>()
            {
                message = "Product not found!",
                statusCode = 404,
            };
        ProductDiscount discount = entity.MapTo<ProductDiscount>();
        discount.productId = productId;
        try
        {
            var result = await _base.InsertAsync(discount, ct: ct);
            return new Response<ProductDiscountDto>()
            {
                result = mapper.Map<ProductDiscountDto>(result),
                statusCode = 200,
                message = "OK"
            };
        }
        catch (Exception ex)
        {
            return new Response<ProductDiscountDto>()
            {
                errors = [ex.Message],
                statusCode = ex.Message.Contains("not found") ? 404 : 400,
                message = "Failed"
            };
        }
    }

 


    public async Task<PaginatedResponse<ProductDiscountDto>> GetAllDiscountsAsync(ProductDiscountQuery query,
        PaginationQuery pagination, CancellationToken ct = default)
    {
        var (result, total) = await _base.GetAsync<ProductDiscountQuery, ProductDiscount>(query, pagination, ct);
        return new PaginatedResponse<ProductDiscountDto>()
        {
            result = mapper.Map<List<ProductDiscountDto>>(result),
            statusCode = 200,
            page = pagination.page,
            pageSize = pagination.pageSize,
            total = total,
            message = "OK",
        };
    }

    public async Task<Response<ProductDiscountDto>> DeleteDiscountsAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var result = await _base.DeleteAsync<Guid, ProductDiscount>(id, ct);
            return new Response<ProductDiscountDto>()
            {
                result = mapper.Map<ProductDiscountDto>(result),
                statusCode = 200,
                message = "OK"
            };
        }
        catch (Exception ex)
        {
            return new Response<ProductDiscountDto>()
            {
                errors = [ex.Message],
                statusCode = ex.Message.Contains("not found") ? 404 : 400,
                message = "Failed"
            };
        }
    }
}