using System.ComponentModel.DataAnnotations.Schema;

namespace MurabaDemo.Database.Tables.Infrastructure;

public class FullAuditedEntitiy<T> : BaseEntity<T> where T : struct 
{
    [ForeignKey(nameof(createdBy))]
    public T createdById { get; set; }
    public virtual  User createdBy { get; set; }
   
    [ForeignKey(nameof(updatedBy))]
    public T? updatedById { get; set; }
    public virtual  User? updatedBy { get; set; }
    
    [ForeignKey(nameof(deletedBy))]
    public T? deletedById { get; set; }
    public virtual User? deletedBy { get; set; }
}