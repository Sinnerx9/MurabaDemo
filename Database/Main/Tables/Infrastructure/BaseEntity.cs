using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MurabaDemo.Database.Tables.Infrastructure;

public class BaseEntity<T>  where T : struct 
{  

    [Key] public T id { get; set; }

    public bool isDeleted { get; set; }
    public DateTime createdAt { get; set; }
    public DateTime updatedAt { get; set; }
}