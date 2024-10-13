using Microsoft.EntityFrameworkCore;
using MurabaDemo.Database.Tables;
using MurabaDemo.Database.Tables.Infrastructure;
using MurabaDemo.Helpers.Interceptors;

namespace MurabaDemo.Database.Main;

public class MainDbContext(DbContextOptions<MainDbContext> options, IHttpContextAccessor httpContextAccessor) : DbContext(options)
{
    public DbSet<Product> products { get; set; }
    public DbSet<ProductDiscount> productDiscounts { get; set; }
    public DbSet<Invoice> invoices { get; set; }
    public DbSet<InvoiceItem> invoiceItems { get; set; }
    public DbSet<User> users { get; set; }



    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(new TimestampInterceptor());
        optionsBuilder.AddInterceptors(new FullAuditedInterceptor(httpContextAccessor));

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

     
    }


}