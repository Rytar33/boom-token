using Application.Interfaces.Options;

namespace Infrastructure.Options;

public class JwtOption : IJwtOption
{
    public string Issure { get; set; }

    public string Audience { get; set; }

    public string Expires { get; set; }

    public string SecretKey { get; set; }
}
