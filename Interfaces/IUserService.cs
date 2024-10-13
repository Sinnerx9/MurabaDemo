using MurabaDemo.Models.Dtos.User;
using MurabaDemo.Models.Infrastructure;
using MurabaDemo.Models.Queries;

namespace MurabaDemo.Interfaces;

public interface IUserService
{
    public Task<Response<UserDto>> InsertAsync(UserForm entity, bool transaction = false,
        CancellationToken ct = default);

    public Task<Response<UserDto>> UpdateAsync(Guid id, UserForm entity, bool transaction = false,
        CancellationToken ct = default);

    public Task<PaginatedResponse<UserDto>> GetAsync(UserQuery query, PaginationQuery pagination,
        CancellationToken ct = default);

    public Task<Response<UserDto>> ShowAsync(Guid id, CancellationToken ct = default);

    public Task<Response<UserDto>> DeleteAsync(Guid id, CancellationToken ct = default);

    public Task<Response<UserDto>> Login(AuthForm auth, CancellationToken ct = default);
}