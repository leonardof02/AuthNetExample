using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

public static class JwtAuthenticationExtension
{
    public static AuthenticationBuilder AddJwtAuthentication(
        this AuthenticationBuilder builder,
        IConfiguration configuration
    )
    {
        var settings = configuration.GetSection("JwtSettings").Get<JwtSettings>()!;

        return builder
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = settings.Issuer,
                ValidAudience = settings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SecretKey))
            };
        });
    }
}