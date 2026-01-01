using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Jobs.Backend.Features.Auth.Models;
using Features.Shared.Persistence;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

public record GeneratedToken(string JwtToken, string RefreshToken, DateTime ExpiresAt);

public class TokenService
{
    private JwtSettings _jwtSettings;
    private readonly ApplicationDbContext _dbContext;


    public TokenService(
        ApplicationDbContext dbContext,
        IOptions<JwtSettings> jwtSettings
    )
    {
        _dbContext = dbContext;
        _jwtSettings = jwtSettings.Value;
    }

    private async Task<string> CreateRefreshTokenAsync(string userId)
    {
        var refreshToken = new RefreshToken(userId);

        _dbContext.RefreshTokens.Add(refreshToken);
        await _dbContext.SaveChangesAsync();

        return refreshToken.Token;
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string jwtToken)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(jwtToken, tokenValidationParameters, out SecurityToken securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }

    public async Task<bool> IsValidRefreshTokenAsync(string token, string userId)
    {
        await RevokeRefreshTokenAsync(token);
        var refreshToken = await _dbContext.RefreshTokens.FindAsync(token);
        return refreshToken != null && refreshToken.UserId == userId && refreshToken.ExpiresAt > DateTime.UtcNow;
    }

    private async Task RevokeRefreshTokenAsync(string token)
    {
        var refreshToken = await _dbContext.RefreshTokens.FindAsync(token);
        if (refreshToken != null)
        {
            _dbContext.RefreshTokens.Remove(refreshToken);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task<GeneratedToken> GenerateJwtToken(ApplicationUser user, IList<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds
        );

        var refreshToken = await CreateRefreshTokenAsync(user.Id);

        return new GeneratedToken(
            new JwtSecurityTokenHandler().WriteToken(token),
            refreshToken,
            token.ValidTo
        );
    }
}