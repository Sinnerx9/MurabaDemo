using MurabaDemo.Models.Dtos.Reports;
using MurabaDemo.Models.Infrastructure;
using MurabaDemo.Models.Queries;

namespace MurabaDemo.Interfaces;

public interface  IReportService
{
    Task<PaginatedResponse<SalesReport>> SalesReport(SalesReportQuery query, PaginationQuery pagination,
        CancellationToken ct);
    Task<Response<ProfitReport>> ProfitReport(ProfitReportQuery query, CancellationToken ct);
}