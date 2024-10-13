using MurabaDemo.Enums;

namespace MurabaDemo.Models.Queries;

public class UserQuery
{
    public string? name { get; set; }
    public Role? role { get; set; }
}