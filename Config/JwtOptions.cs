namespace JobTracker.api.Config;

public class JwtOptions
{
    public string Key { get; set; } = default!;
    public string Issuer { get; set; } = default!;
    public string Audience { get; set; } = default!;
    public int ExpiresDays { get; set; } = 730; // ~2 ans (730 jours)
}
