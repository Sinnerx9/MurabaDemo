using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MurabaDemo.Controllers.Infrastructure;
using MurabaDemo.Database.Tables;
using MurabaDemo.Interfaces;
using MurabaDemo.Models.Dtos.Invoice;
using MurabaDemo.Models.Infrastructure;
using MurabaDemo.Models.Queries;
namespace MurabaDemo.Controllers;


[Authorize]
public class OrderController(IOrderService orderService) : BaseController
{
    [HttpPost]
    public async Task<IActionResult> InsertAsync([FromBody] InvoiceForm orderForm,
        CancellationToken ct = default)
    {
        var response = await orderService.InsertAsync(orderForm, ct: ct);
        return StatusCode(response.statusCode, response);
    }
    

    [HttpGet]
    public async Task<IActionResult> GetAsync([FromQuery] InvoiceQuery orderQuery,
        [FromQuery] PaginationQuery paginationQuery, CancellationToken ct = default)
    {
        var response = await orderService.GetAsync(orderQuery, paginationQuery, ct);
        return StatusCode(response.statusCode, response);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ShowAsync(Guid id, CancellationToken ct = default)
    {
        var response = await orderService.ShowAsync(id, ct);
        return StatusCode(response.statusCode, response);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var response = await orderService.DeleteAsync(id, ct);
        return StatusCode(response.statusCode, response);
    }
}