namespace AuthNetExample.Features.Auth.Models.Params;

public record UserRole : IParsable<UserRole>
{
    
    public static readonly UserRole Recruiter = new("recruiter");
    public static readonly UserRole ApplicationUser = new("user");

    public string Value { get; init; }

    private UserRole(string value) => Value = value;

    public static UserRole Parse(string s, IFormatProvider? provider)
    {
        if (TryParse(s, provider, out var result)) return result;
        throw new FormatException($"Invalid Role: {s}");
    }

    public static bool TryParse(string? s, IFormatProvider? provider, out UserRole result)
    {
        result = s?.ToLower() switch
        {
            "recruiter" => Recruiter,
            "user" => ApplicationUser,
            _ => null!
        };
        return result != null;
    }

    public override string ToString() => Value;
}