namespace MurabaDemo.Models.Infrastructure;

public class BaseEntityDto<T> where T : struct
{
    public T id { get; set; }

    public bool isDeleted { get; set; }
    public DateTime createdAt { get; set; }
    public DateTime updatedAt { get; set; }
}