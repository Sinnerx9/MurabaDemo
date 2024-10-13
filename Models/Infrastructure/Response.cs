using System.Text.Json.Serialization;

namespace MurabaDemo.Models.Infrastructure;

public class Response<T>
{
    public T? result { get; set; }
    public string? message { get; set; }
    [JsonIgnore] public int statusCode { get; set; }
    public bool hasError { get; set; }
    public List<string>? errors { get; set; }
}