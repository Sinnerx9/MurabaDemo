using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MurabaDemo.Database.Main;
using MurabaDemo.Database.Tables;
using MurabaDemo.Helpers.Extensions;
using MurabaDemo.Interfaces;
using MurabaDemo.Models.Dtos.Invoice;
using MurabaDemo.Models.Infrastructure;
using MurabaDemo.Models.Queries;

namespace MurabaDemo.Services;

public class OrderService(IBaseService _base, MainDbContext context, IMapper mapper) : IOrderService
{
    public async Task<Response<InvoiceDto>> InsertAsync(InvoiceForm entity, bool transaction = false,
        CancellationToken ct = default)
    {
        Invoice invoice = mapper.Map<Invoice>(entity);
        invoice.invoiceDate = DateTime.Now;
        for (int i = 0; i < invoice.items.Count; i++)
        {
            var product = await _base.ShowAsync<Guid, Product>(invoice.items.ElementAt(i).productId, ct);
            if (product == null)
                throw new Exception("product not found");
            var lastDiscount = await context.productDiscounts.Where(x => x.productId == product.id)
                .OrderByDescending(x => x.createdAt).FirstOrDefaultAsync(ct);
            if (lastDiscount != null)
                invoice.items.ElementAt(i).discountId = lastDiscount.id;
        }

        try
        {
            var result = await _base.InsertAsync(invoice, ct: ct);
            return new Response<InvoiceDto>()
            {
                result = mapper.Map<InvoiceDto>(result),
                statusCode = 200,
                message = "OK"
            };
        }
        catch (Exception ex)
        {
            return new Response<InvoiceDto>()
            {
                errors = [ex.Message],
                statusCode = ex.Message.Contains("not found") ? 404 : 400,
                message = "Failed"
            };
        }
    }

    public async Task<Response<InvoiceDto>> UpdateAsync(Guid id, InvoiceForm entity, bool transaction = false,
        CancellationToken ct = default)
    {
        try
        {
            var result = await _base.UpdateAsync<Guid, Invoice, InvoiceForm>(id, entity, ct: ct);
            return new Response<InvoiceDto>()
            {
                result = mapper.Map<InvoiceDto>(result),
                statusCode = 200,
                message = "OK"
            };
        }
        catch (Exception ex)
        {
            return new Response<InvoiceDto>()
            {
                errors = [ex.Message],
                statusCode = ex.Message.Contains("not found") ? 404 : 400,
                message = "Failed"
            };
        }
    }

    public async Task<PaginatedResponse<InvoiceDto>> GetAsync(InvoiceQuery query, PaginationQuery pagination,
        CancellationToken ct = default)
    {
        var (result, total) = await _base.GetAsync<InvoiceQuery, Invoice>(query, pagination, ct);
        return new PaginatedResponse<InvoiceDto>()
        {
            result = mapper.Map<List<InvoiceDto>>(result),
            statusCode = 200,
            page = pagination.page,
            pageSize = pagination.pageSize,
            total = total,
            message = "OK",
        };
    }

    public async Task<Response<InvoiceDto>> ShowAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var result = await _base.ShowAsync<Guid, Invoice>(id, ct);
            return new Response<InvoiceDto>()
            {
                result = mapper.Map<InvoiceDto>(result),
                statusCode = 200,
                message = "OK"
            };
        }
        catch (Exception ex)
        {
            return new Response<InvoiceDto>()
            {
                errors = [ex.Message],
                statusCode = ex.Message.Contains("not found") ? 404 : 400,
                message = "Failed"
            };
        }
    }

    public async Task<Response<InvoiceDto>> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var result = await _base.DeleteAsync<Guid, Invoice>(id, ct);
            return new Response<InvoiceDto>()
            {
                result = mapper.Map<InvoiceDto>(result),
                statusCode = 200,
                message = "OK"
            };
        }
        catch (Exception ex)
        {
            return new Response<InvoiceDto>()
            {
                errors = [ex.Message],
                statusCode = ex.Message.Contains("not found") ? 404 : 400,
                message = "Failed"
            };
        }
    }
}