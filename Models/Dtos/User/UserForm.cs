using MurabaDemo.Enums;

namespace MurabaDemo.Models.Dtos.User;

public class UserForm
{
    public string name { get; set; }
    public Role role { get; set; }

    public string password { get; set; }
}