namespace MurabaDemo.Helpers.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class MapToPropertyAttribute : Attribute
{
    public string EntityPropertyName { get; }
    
    public MapToPropertyAttribute(string entityPropertyName)
    {
        EntityPropertyName = entityPropertyName;
    }
}
