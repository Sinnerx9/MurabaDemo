using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MurabaDemo.Database.Main;
using MurabaDemo.Database.Tables;
using MurabaDemo.Models.Infrastructure;

namespace MurabaDemo.Helpers.Extensions;

public static class Utilities
{
    public static bool IsNullOrEmpty(this Guid? guid) => guid == null || guid == Guid.Empty;
    public static bool IsNullOrEmpty(this Guid guid) => guid == Guid.Empty;

    public static TTarget MapTo<TTarget, TSource>(this TTarget target, TSource source) where TTarget : class
        where TSource : class
    {
        var mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<TTarget, TSource>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember, destMember) => destMember == null));
        }));
        mapper.Map(target, source);
        mapper = new(new MapperConfiguration(cfg => cfg.CreateMap<TSource, TTarget>()));
        mapper.Map(source, target);
        return target;
    }

    public static TTarget MapTo<TTarget>(this object source) where TTarget : class, new()
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source), "Source object cannot be null");
        }

        var mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.CreateMap(source.GetType(), typeof(TTarget))
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember, destMember) => 
                {
                    if (srcMember == null)
                        return false;

                    if (destMember is decimal || destMember is DateTime || destMember is Enum)
                        return true;
                    return destMember == null;
                }));
        }));

        var target = new TTarget();
        mapper.Map(source, target);
        return target;
    }



    public static TTarget CopyChangesFrom<TTarget, TRDTO>(this TTarget original, TRDTO newobj)
        where TTarget : class where TRDTO : class
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<TRDTO, TTarget>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember, destMember) =>
                {
                    if (srcMember == null) return false;

                    // Handle default values
                    if (srcMember.Equals(GetDefault(srcMember.GetType()))) return false;

                    return true;
                }));
        });

        var mapper = new Mapper(config);
        mapper.Map(newobj, original);

        return original;
    }

    private static object GetDefault(Type type)
    {
        return type.IsValueType ? Activator.CreateInstance(type) : null;
    }

    public static ObjectResult ToDefaultResponse<T>(this ControllerBase controller, Response<T> response)
        where T : class
    {
        return controller.StatusCode(response.statusCode, response);
    }


    // public static async Task<User?> GetCurrentUserAsync<TID>(this IHttpContextAccessor _accessor, MainDbContext _context)
    // {
    //     var userId = _accessor.GetId<TID>();
    //     return await _context.users.FirstOrDefaultAsync(x => x.id == userId);
    // }


    public static IActionResult SuccessResponse<T>(this ControllerBase controller, T? data = null,
        string? message = null) where T : class
    {
        return controller.ToDefaultResponse(new Response<T>
        {
            statusCode = 200,
            hasError = false,
            result = data,
            message = message
        });
    }

    public static IActionResult NotFoundResponse(this ControllerBase controller, string message)
    {
        return controller.ToDefaultResponse(new Response<dynamic>
        {
            hasError = true,
            statusCode = 404,
            errors = new List<string>() { message }
        });
    }

    public static IActionResult BadRequestResponse(this ControllerBase controller, string message)
    {
        return controller.ToDefaultResponse(new Response<dynamic>
        {
            hasError = true,
            statusCode = 400,
            errors = new List<string>() { message }
        });
    }

    public static IActionResult UnauthorizedResponse(this ControllerBase controller, string message)
    {
        return controller.ToDefaultResponse(new Response<dynamic>
        {
            hasError = true,
            statusCode = 401,
            errors = new List<string>() { message }
        });
    }

    public static IActionResult ForbiddenResponse(this ControllerBase controller, string message)
    {
        return controller.ToDefaultResponse(new Response<dynamic>
        {
            hasError = true,
            statusCode = 403,
            errors = new List<string>() { message }
        });
    }

    public static ObjectResult ToDefaultResponse<T>(this ControllerBase controller, PaginatedResponse<T> response)
        where T : class
    {
        return controller.StatusCode(response.statusCode, response);
    }

    public static T? GetId<T>(this IHttpContextAccessor accessor) where T : struct
    {
        try
        {
            if (accessor.HttpContext != null)
            {
                var idString = accessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)?.ToString();
                MethodInfo? parseMethod = typeof(T).GetMethod("Parse", new[] { typeof(string) });
                if (parseMethod != null)
                {
                    try
                    {
                        T? result = (T?)parseMethod.Invoke(null, new object[] { idString });
                        if (result == null)
                            throw new InvalidOperationException($"Failed to parse '{idString}' to {typeof(T)}");

                        return result.Value;
                    }
                    catch (TargetInvocationException ex)
                    {
                        throw new InvalidOperationException($"Failed to parse '{idString}' to {typeof(T)}", ex);
                    }
                }
                else
                {
                    throw new InvalidOperationException($"{typeof(T)} does not have a Parse method.");
                }
            }

            else
            {
                throw new InvalidOperationException("ID not found in HttpContext.Items.");
            }
        }
        catch

        {
            return null;
        }
    }

    public static ExpandoObject ToExpando(this object source)
    {
        var expando = new ExpandoObject() as IDictionary<string, Object>;
        foreach (var property in source.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            expando.Add(property.Name, property.GetValue(source));
        }

        return (ExpandoObject)expando;
    }

    public static ExpandoObject AddProperty(this ExpandoObject expando, string propertyName, object value)
    {
        var expandoDict = expando as IDictionary<string, object>;
        expandoDict[propertyName] = value;
        return expando;
    }


    public static string ToBase64(this string data) => Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
    public static string FromBase64(this string data) => Encoding.UTF8.GetString(Convert.FromBase64String(data));
}