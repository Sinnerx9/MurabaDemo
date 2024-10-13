using System.ComponentModel.DataAnnotations.Schema;
using MurabaDemo.Database.Tables.Infrastructure;
using MurabaDemo.Enums;

namespace MurabaDemo.Database.Tables;

[Table("users")]
public class User : BaseEntity<Guid>
{
    public string name { get; set; }
    public Role role { get; set; }
    public string password { get; set; }
}