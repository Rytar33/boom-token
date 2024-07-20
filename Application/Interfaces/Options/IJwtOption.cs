namespace Application.Interfaces.Options;

public interface IJwtOption
{
    string Issure { get; set; }

    string Audience { get; set; }

    string Expires { get; set; }

    string SecretKey { get; set; }
}
