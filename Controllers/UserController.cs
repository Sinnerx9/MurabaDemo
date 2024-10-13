using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MurabaDemo.Controllers.Infrastructure;
using MurabaDemo.Interfaces;
using MurabaDemo.Models.Dtos.User;
using MurabaDemo.Models.Infrastructure;
using MurabaDemo.Models.Queries;

namespace MurabaDemo.Controllers;

[Authorize(Roles= "ADMIN")]
public class UserController(IUserService userService) : BaseController
{
    [AllowAnonymous]
    [HttpPost("auth")]
    public async Task<IActionResult> LoginAsync([FromBody] AuthForm authForm,
        CancellationToken ct = default)
    {
        var response = await userService.Login(authForm, ct: ct);
        return StatusCode(response.statusCode, response);
    }
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> InsertAsync([FromBody] UserForm userForm,
        CancellationToken ct = default)
    {
        var response = await userService.InsertAsync(userForm, ct: ct);
        return StatusCode(response.statusCode, response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UserForm userForm,
        CancellationToken ct = default)
    {
        var response = await userService.UpdateAsync(id, userForm, ct: ct);
        return StatusCode(response.statusCode, response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync([FromQuery] UserQuery userQuery,
        [FromQuery] PaginationQuery paginationQuery, CancellationToken ct = default)
    {
        var response = await userService.GetAsync(userQuery, paginationQuery, ct);
        return StatusCode(response.statusCode, response);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ShowAsync(Guid id, CancellationToken ct = default)
    {
        var response = await userService.ShowAsync(id, ct);
        return StatusCode(response.statusCode, response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var response = await userService.DeleteAsync(id, ct);
        return StatusCode(response.statusCode, response);
    }
}