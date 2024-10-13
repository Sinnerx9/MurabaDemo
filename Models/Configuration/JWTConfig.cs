namespace MurabaDemo.Models.Configuration;

public class JWTConfig
{
    public string Secret { get; init; } = null!;
    public int ExpireDays { get; init; }
    public string Issuer { get; init; } = null!;
    public string Audience { get; init; } = null!;
}