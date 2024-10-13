using Microsoft.EntityFrameworkCore;

namespace MurabaDemo.Database.Log;

public class LogDbContext : DbContext
{
   // public DbSet<LogEntry> LogEntries { get; set; }
   public LogDbContext(DbContextOptions<LogDbContext> options) : base(options)
   {
      
   }
}