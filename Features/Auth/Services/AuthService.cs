using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using AuthNetExample.Features.Auth.Models;
using AuthNetExample.Features.Shared.Core;
using AuthNetExample.Features.Auth.Errors;
using System.Security.Cryptography;
using Features.Auth.Models.Requests.RegisterRequest;
using Features.Shared.Persistence;

namespace AuthNetExample.Features.Auth.Services;

public class AuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly JwtSettings _jwtSettings;

    private readonly ApplicationDbContext _dbContext;

    public AuthService(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IOptions<JwtSettings> jwtSettings,
        ApplicationDbContext dbContext
    )
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtSettings = jwtSettings.Value;
        _dbContext = dbContext;
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            return Result.Failure<AuthResponse>(new LoginError
            {
                Message = "Invalid email or password."
            });
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

        if (!result.Succeeded)
        {
            return Result.Failure<AuthResponse>(new LoginError
            {
                Message = "Invalid email or password."
            });
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = GenerateJwtToken(user, roles);
        var refreshToken = GenerateRefreshToken(user.Id);
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);
        
        _dbContext.RefreshTokens.Add(refreshToken);
        await _dbContext.SaveChangesAsync();

        return Result.Success(new AuthResponse
        {
            Token = token,
            Email = user.Email!,
            ExpiresAt = expiresAt,
            RefreshToken = refreshToken.Token
        });
    }

    public async Task<Result<AuthResponse>> RefreshTokenAsync(string token)
    {
        var refreshToken = await _dbContext.RefreshTokens.FindAsync(token);

        if (refreshToken == null || !refreshToken.IsActive)
        {
            return Result.Failure<AuthResponse>(new RefreshTokenError
            {
                Message = "Invalid or expired refresh token."
            });
        }

        var user = await _userManager.FindByIdAsync(refreshToken.UserId.ToString());
        
        if (user == null)
        {
            return Result.Failure<AuthResponse>(new RefreshTokenError
            {
                Message = "User not found."
            });
        }

        var roles = await _userManager.GetRolesAsync(user);
        var newJwtToken = GenerateJwtToken(user, roles);
        var newRefreshToken = GenerateRefreshToken(user.Id);
        
        refreshToken.Revoke();

        _dbContext.RefreshTokens.Add(newRefreshToken);
        await _dbContext.SaveChangesAsync();

        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);

        return Result.Success(new AuthResponse
        {
            Token = newJwtToken,
            Email = user.Email!,
            ExpiresAt = expiresAt,
            RefreshToken = newRefreshToken.Token
        });
    }

    public async Task<Result<AuthResponse>> SignInWithGithubAsync(string email, string nameIdentifier) {
        
        if (email == null || nameIdentifier == null)
        {
            return Result.Failure<AuthResponse>(new ExternalAuthError
            {
                Message = "Email or NameIdentifier claim not found."
            });
        }

        var existingUser = await _userManager.FindByLoginAsync("GitHub", nameIdentifier);
        
        if (existingUser == null)
        {
            existingUser = await _userManager.FindByEmailAsync(email);

            if (existingUser == null)
            {
                existingUser = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var createResult = await _userManager.CreateAsync(existingUser);

                if (!createResult.Succeeded)
                {
                    return Result.Failure<AuthResponse>(new ExternalAuthError
                    {
                        Message = "Error creating user."
                    });
                }
            }

            var loginInfo = new UserLoginInfo("GitHub", nameIdentifier, "GitHub");
            var addLoginResult = await _userManager.AddLoginAsync(existingUser, loginInfo);

            if (!addLoginResult.Succeeded)
            {
                return Result.Failure<AuthResponse>(new ExternalAuthError
                {
                    Message = "Error adding external login."
                });
            }
        }

        var roles = await _userManager.GetRolesAsync(existingUser);
        var token = GenerateJwtToken(existingUser, roles);
        var refreshToken = GenerateRefreshToken(existingUser.Id);
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);

        _dbContext.RefreshTokens.Add(refreshToken);
        await _dbContext.SaveChangesAsync();

        return Result.Success(new AuthResponse
        {
            Token = token,
            Email = existingUser.Email!,
            ExpiresAt = expiresAt,
            RefreshToken = refreshToken.Token
        });
    }

    private string GenerateJwtToken(IdentityUser user, IList<string>? roles = null)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        // Add roles to claims if provided
        if (roles != null && roles.Any())
        {
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        }

        claims.Add(new Claim(ClaimTypes.Name, user.UserName ?? ""));
        claims.Add(new Claim("onboarding_completed", user is ApplicationUser appUser ? appUser.IsOnboardingCompleted.ToString() : "false"));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<Result<AuthResponse>> RegisterUserAsync(RegisterUserRequest request)
    {
        var newUser = new ApplicationUser
        {
            UserName = request.Username,
            Email = request.Email,
            EmailConfirmed = true,
            IsOnboardingCompleted = false
        };

        await _userManager.CreateAsync(newUser);
        await _userManager.AddToRoleAsync(newUser, request.Role.ToString());

        var token = GenerateJwtToken(newUser, new List<string> { request.Role });
        var refreshToken = GenerateRefreshToken(newUser.Id);
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);

        _dbContext.RefreshTokens.Add(refreshToken);
        await _dbContext.SaveChangesAsync();
        
        return Result.Success(new AuthResponse
        {
            Token = token,
            Email = newUser.Email!,
            ExpiresAt = expiresAt,
            RefreshToken = refreshToken.Token
        });
    }


    private RefreshToken GenerateRefreshToken(string userId)
    {
        var refreshToken = new RefreshToken(userId: userId)
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
        };
        return refreshToken;
    }

    // Public methods for external authentication
    public async Task<(string Token, DateTime ExpiresAt)> GenerateJwtTokenForUser(IdentityUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var token = GenerateJwtToken(user, roles);
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);
        return (token, expiresAt);
    }

    public async Task<string> GenerateAndSaveRefreshToken(string userId)
    {
        var refreshToken = GenerateRefreshToken(userId);
        _dbContext.RefreshTokens.Add(refreshToken);
        await _dbContext.SaveChangesAsync();
        return refreshToken.Token;
    }
}