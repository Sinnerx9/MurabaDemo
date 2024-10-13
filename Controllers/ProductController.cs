using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MurabaDemo.Controllers.Infrastructure;
using MurabaDemo.Database.Tables;
using MurabaDemo.Interfaces;
using MurabaDemo.Models.Dtos.Product;
using MurabaDemo.Models.Dtos.ProductDiscount;
using MurabaDemo.Models.Infrastructure;
using MurabaDemo.Models.Queries;

namespace MurabaDemo.Controllers;

[Authorize(Roles = "ADMIN")]
public class ProductController(IProductService productService) : BaseController
{
    [HttpPost]
    public async Task<IActionResult> InsertAsync([FromBody] ProductForm productForm,
        CancellationToken ct = default)
    {
        var response = await productService.InsertAsync(productForm, ct: ct);
        return StatusCode(response.statusCode, response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] ProductForm productForm,
        CancellationToken ct = default)
    {
        var response = await productService.UpdateAsync(id, productForm, ct: ct);
        return StatusCode(response.statusCode, response);
    }

    [HttpPost("{productId:guid}/Discount")]
    public async Task<IActionResult> InsertDiscountAsync([FromRoute] Guid productId,
        [FromBody] ProductDiscountForm productDiscount,
        CancellationToken ct = default)
    {
        var response = await productService.InsertDiscountAsync(productId, productDiscount, ct: ct);
        return StatusCode(response.statusCode, response);
    }

    [HttpDelete("Discount/{id:guid}")]
    public async Task<IActionResult> DeleteDiscountsAsync([FromRoute] Guid id,
        CancellationToken ct = default)
    {
        var response = await productService.DeleteDiscountsAsync(id, ct: ct);
        return StatusCode(response.statusCode, response);
    }

    [HttpGet("Discounts")]
    public async Task<IActionResult> GetAllDiscountsAsync([FromQuery] ProductDiscountQuery query, [FromQuery] PaginationQuery pagination,
        CancellationToken ct = default)
    {
        var response = await productService.GetAllDiscountsAsync(query, pagination, ct);
        return StatusCode(response.statusCode, response);
    }


    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAsync([FromQuery] ProductQuery productQuery,
        [FromQuery] PaginationQuery paginationQuery, CancellationToken ct = default)
    {
        var response = await productService.GetAsync(productQuery, paginationQuery, ct);
        return StatusCode(response.statusCode, response);
    }


    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ShowAsync(Guid id, CancellationToken ct = default)
    {
        var response = await productService.ShowAsync(id, ct);
        return StatusCode(response.statusCode, response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var response = await productService.DeleteAsync(id, ct);
        return StatusCode(response.statusCode, response);
    }
}