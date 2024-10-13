using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MurabaDemo.Database.Main;
using MurabaDemo.Database.Tables;
using MurabaDemo.Enums;
using MurabaDemo.Helpers.Extensions;
using MurabaDemo.Interfaces;
using MurabaDemo.Models.Configuration;
using MurabaDemo.Models.Dtos.User;
using MurabaDemo.Models.Infrastructure;
using MurabaDemo.Models.Queries;
using iBCrypt = BCrypt.Net.BCrypt;
namespace MurabaDemo.Services;

public class UserService(IBaseService _base, IMapper mapper, MainDbContext context,  IOptions<JWTConfig> config) : IUserService
{
    public async Task<Response<UserDto>> InsertAsync(UserForm entity, bool transaction = false,
        CancellationToken ct = default)
    {
        entity.password = iBCrypt.HashPassword(entity.password);
        User entitiy = entity.MapTo<User>();
        try
        {
            
            var result = await _base.InsertAsync(entitiy, ct: ct);
            return new Response<UserDto>()
            {
                result = mapper.Map<UserDto>(result),
                statusCode = 200,
                message = "OK"
            };
        }
        catch (Exception ex)
        {
            return new Response<UserDto>()
            {
                errors = [ex.Message],
                statusCode = ex.Message.Contains("not found") ? 404 : 400,
                message = "Failed"
            };
        }
    }

    public async Task<Response<UserDto>> UpdateAsync(Guid id, UserForm entity, bool transaction = false,
        CancellationToken ct = default)
    {
        try
        {
            entity.password = iBCrypt.HashPassword(entity.password);
            var result = await _base.UpdateAsync<Guid, User, UserForm>(id, entity, ct: ct);
            return new Response<UserDto>()
            {
                result = mapper.Map<UserDto>(result),
                statusCode = 200,
                message = "OK"
            };
        }
        catch (Exception ex)
        {
            return new Response<UserDto>()
            {
                errors = [ex.Message],
                statusCode = ex.Message.Contains("not found") ? 404 : 400,
                message = "Failed"
            };
        }
    }

    public async Task<PaginatedResponse<UserDto>> GetAsync(UserQuery query, PaginationQuery pagination,
        CancellationToken ct = default)
    {
        var (result, total) = await _base.GetAsync<UserQuery, User>(query, pagination, ct);
        return new PaginatedResponse<UserDto>()
        {
            message = "OK",
            result = mapper.Map<List<UserDto>>(result),
            statusCode = 200,
            page = pagination.page,
            pageSize = pagination.pageSize,
            total = total
        };
    }

    public async Task<Response<UserDto>> ShowAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var result = await _base.ShowAsync<Guid, User>(id, ct);
            return new Response<UserDto>()
            {
                result = mapper.Map<UserDto>(result),
                statusCode = 200,
                message = "OK"
            };
        }
        catch (Exception ex)
        {
            return new Response<UserDto>()
            {
                errors = [ex.Message],
                statusCode = ex.Message.Contains("not found") ? 404 : 400,
                message = "Failed"
            };
        }
    }

    public async Task<Response<UserDto>> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var result = await _base.DeleteAsync<Guid, User>(id, ct);
            return new Response<UserDto>()
            {
                result = mapper.Map<UserDto>(result),
                statusCode = 200,
                message = "OK"
            };
        }
        catch (Exception ex)
        {
            return new Response<UserDto>()
            {
                errors = [ex.Message],
                statusCode = ex.Message.Contains("not found") ? 404 : 400,
                message = "Failed"
            };
        }
    }

    public async Task<Response<UserDto>> Login(AuthForm auth, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(auth.identifier))
            return new Response<UserDto>
            {
                statusCode = 403,
                hasError = true,
                errors = ["identifier is required!"]
            };

        var users = context.users.AsNoTracking();
        users = users.Where(x =>
            x.name.ToLower() == auth.identifier.ToLower());


        var userEntity = await users.FirstOrDefaultAsync(ct);
        if (userEntity == null)
            return new Response<UserDto>
            {
                statusCode = 401,
                hasError = true,
                errors = ["Login failed; Invalid user ID or password."]
            };


        if (!iBCrypt.Verify(auth.password, userEntity.password))
            return new Response<UserDto>
            {
                statusCode = 401,
                hasError = true,
                errors = ["Login failed; Invalid user ID or password."]
            };

        var result = mapper.Map<UserDto>(userEntity);
        result.token = GenerateToken(userEntity.id, userEntity.role);
        return new Response<UserDto>
        {
            result = result,
            statusCode = 200,
            message = "Login Success",
            hasError = true,
        };
    }


    private string GenerateToken(Guid id, Role role)
    {
        var _config = config.Value;
        var handler = new JwtSecurityTokenHandler();
        var desc = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
                new Claim[]
                {
                    new(ClaimTypes.NameIdentifier, id.ToString()),
                    new(ClaimTypes.Role, role.ToString())
                }),
            Expires = DateTime.Now.AddDays(_config.ExpireDays),
            Issuer = _config.Issuer,
            Audience = _config.Audience,
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Secret)),
                    SecurityAlgorithms.HmacSha256)
        };
        var token = handler.CreateToken(desc);
        return handler.WriteToken(token);
    }
}