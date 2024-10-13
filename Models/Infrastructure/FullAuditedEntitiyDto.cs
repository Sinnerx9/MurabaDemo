using MurabaDemo.Models.Dtos.User;

namespace MurabaDemo.Models.Infrastructure;

public class FullAuditedEntitiyDto<T> : BaseEntityDto<T> where T : struct 
{
    public virtual  UserDto createdBy { get; set; }
   
    public virtual  UserDto? updatedBy { get; set; }
    
    public virtual UserDto? deletedBy { get; set; }
}