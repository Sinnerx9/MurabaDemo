using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MurabaDemo.Database.Main;
using MurabaDemo.Enums;
using MurabaDemo.Helpers.Extensions;
using MurabaDemo.Interfaces;
using MurabaDemo.Models.Dtos.Product;
using MurabaDemo.Models.Dtos.Reports;
using MurabaDemo.Models.Infrastructure;
using MurabaDemo.Models.Queries;

namespace MurabaDemo.Services;

public class ReportService(MainDbContext context, IMapper mapper) : IReportService
{
    public async Task<PaginatedResponse<SalesReport>> SalesReport(SalesReportQuery query, PaginationQuery pagination,
        CancellationToken ct)
    {
        var salesQuery = context.invoices.DynamicSearch(query);

        if (query.productId != null && query.productId != Guid.Empty)
        {
            salesQuery = salesQuery.Where(x => x.items.Any(item => item.productId == query.productId));
        }

        var filteredItemsQuery = salesQuery.SelectMany(x => x.items)
            .Where(item => query.productId == null || item.productId == query.productId);

        var groupedSalesQuery = filteredItemsQuery
            .GroupBy(item => item.product)
            .Select(g => new SalesReport()
            {
                product = mapper.Map<ProductDto>(g.Key),
                totalQuantitySold = g.Sum(item => item.quantity),
                totalAmount = g.Sum(item => item.quantity * g.Key.price), 
                totalDiscountAmount = g.Sum(item => 
                    item.discount != null 
                        ? (item.discount.type == DiscountValueType.FIXED 
                            ? item.quantity * item.discount.discount
                            : (item.discount.discount / 100) * (item.quantity * g.Key.price))
                        : 0)
            });

        var totalCount = await groupedSalesQuery.CountAsync(ct);

        var paginatedSalesReport = await groupedSalesQuery
            .Skip((pagination.page - 1) * pagination.pageSize)
            .Take(pagination.pageSize)
            .ToListAsync(ct);

        return new PaginatedResponse<SalesReport>()
        {
            pageSize = pagination.pageSize,
            page = pagination.page,
            total = totalCount,
            result = paginatedSalesReport,
            message = "OK",
            statusCode = 200,
        };
    }


    public async Task<Response<ProfitReport>> ProfitReport(ProfitReportQuery query, CancellationToken ct)
    {
        var invoiceQuery = context.invoices.DynamicSearch(query);

        

        var totalAmounts = await invoiceQuery.SumAsync(x => x.items.Sum(item => item.quantity * item.product.price), ct);
    
        var totalDiscounts = await invoiceQuery.SumAsync(x => x.items.Sum(item => 
            item.discount != null 
                ? (item.discount.type == DiscountValueType.FIXED 
                    ? item.quantity * item.discount.discount
                    : (item.discount.discount / 100) * (item.quantity * item.product.price)) 
                : 0), ct);
    
        var netAmounts = totalAmounts - totalDiscounts;

        var profitReport = new ProfitReport()
        {
            totalAmounts = totalAmounts,
            totalDiscount = totalDiscounts,
            netAmounts = netAmounts
        };

        return new Response<ProfitReport>()
        {
            result = profitReport,
            message = "Profit report generated successfully",
            statusCode = 200
        };
    }

}