using Microsoft.AspNetCore.Mvc;
using MurabaDemo.Controllers.Infrastructure;
using MurabaDemo.Interfaces;
using MurabaDemo.Models.Infrastructure;
using MurabaDemo.Models.Queries;

namespace MurabaDemo.Controllers;

public class ReportController(IReportService reportService, IProductService productService) : BaseController
{
    [HttpGet("Profit")]
    public async Task<IActionResult> GetAllDiscountsAsync([FromQuery] ProfitReportQuery query,
        CancellationToken ct = default)
    {
        var response = await reportService.ProfitReport(query, ct);
        return StatusCode(response.statusCode, response);
    }

    [HttpGet("Discounts")]
    public async Task<IActionResult> DiscountsReport([FromQuery] ProductDiscountQuery query,
        [FromQuery] PaginationQuery pagination,
        CancellationToken ct = default)
    {
        var response = await productService.GetAllDiscountsAsync(query, pagination, ct);
        return StatusCode(response.statusCode, response);
    }

    [HttpGet("Sales")]
    public async Task<IActionResult> SalesReport([FromQuery] SalesReportQuery query,
        [FromQuery] PaginationQuery pagination,
        CancellationToken ct = default)
    {
        var response = await reportService.SalesReport(query, pagination, ct);
        return StatusCode(response.statusCode, response);
    }
}