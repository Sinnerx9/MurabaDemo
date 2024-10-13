using System.ComponentModel.DataAnnotations;

namespace MurabaDemo.Database.Log.Tables;

public class LogEntry
{
    [Key]
    public Guid id { get; set; }
    
    public string entityName { get; set; }
    public string action { get; set; }
    public string actionBy { get; set; }
    public DateTime actionDate { get; set; }
    public string oldValues { get; set; }
    public string newValues { get; set; }

    
}