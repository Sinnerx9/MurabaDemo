namespace MurabaDemo.Models.Configuration;

public class CORSConfig
{
    public List<string> Origins { get; set; } = null!;
    public bool AllowAnyMethod { get; set; }
    public bool AllowAnyHeader { get; set; }
    public bool AllowCredentials { get; set; }
}