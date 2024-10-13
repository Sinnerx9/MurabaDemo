using MurabaDemo.Database.Tables;
using MurabaDemo.Models.Dtos.Invoice;
using MurabaDemo.Models.Infrastructure;
using MurabaDemo.Models.Queries;

namespace MurabaDemo.Interfaces;

public interface IOrderService
{
    public Task<Response<InvoiceDto>> InsertAsync(InvoiceForm entity, bool transaction = false,
        CancellationToken ct = default);

    public Task<Response<InvoiceDto>> UpdateAsync(Guid id, InvoiceForm entity, bool transaction = false,
        CancellationToken ct = default);

    public Task<PaginatedResponse<InvoiceDto>> GetAsync(InvoiceQuery query, PaginationQuery pagination,
        CancellationToken ct = default);

    public Task<Response<InvoiceDto>> ShowAsync(Guid id, CancellationToken ct = default);

    public Task<Response<InvoiceDto>> DeleteAsync(Guid id, CancellationToken ct = default);
}