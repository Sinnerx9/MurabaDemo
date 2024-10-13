namespace MurabaDemo.Models.Configuration;

public class SwaggerConfig
{
    public string Name { get; set; } = null!;
    public string DocPath { get; set; } = null!;
    public bool isEnable { get; set; }
    public bool customeStyle { get; set; }
    public bool DarkTheme { get; set; }
    public string Drak_StylePath { get; set; } = null!;
    public bool LightTheme { get; set; }
    public string Light_StylePath { get; set; } = null!;
    public bool Filter { get; set; }
    public bool AutoCollapse { get; set; }
    public bool RequestDuration { get; set; }
    public bool Authentication { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; } 
}