using MurabaDemo.Interfaces;
using MurabaDemo.Services;

namespace MurabaDemo;

public static class DependencyInjection
{
    public static IServiceCollection InjectServices(this IServiceCollection collection)
    {
        collection.AddScoped<IBaseService, BaseService>();
        collection.AddScoped<IOrderService, OrderService>();
        collection.AddScoped<IProductService, ProductService>();
        collection.AddScoped<IReportService,ReportService>();
        collection.AddScoped<IUserService,UserService>();
        
        return collection;
    }
}