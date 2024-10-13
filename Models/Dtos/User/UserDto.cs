using System.Text.Json.Serialization;
using MurabaDemo.Database.Tables.Infrastructure;
using MurabaDemo.Enums;
using MurabaDemo.Models.Infrastructure;

namespace MurabaDemo.Models.Dtos.User;

public class UserDto : BaseEntityDto<Guid>
{
    public string name { get; set; }
    public Role role { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? token { get; set; }
}