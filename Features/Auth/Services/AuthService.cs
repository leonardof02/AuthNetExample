using System.Security.Claims;
using AuthNetExample.Features.Auth.Models;
using AuthNetExample.Features.Auth.Models.Constants;
using Features.Auth.Models.Requests.RegisterRequest;
using Features.Shared.Persistence;
using Microsoft.AspNetCore.Identity;

namespace AuthNetExample.Features.Auth.Services;

public class AuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly TokenService _tokenService;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        TokenService tokenService
        )
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email) ?? throw new Exception("Invalid login attempt.");
        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

        if (!result.Succeeded) throw new Exception("Invalid credentials.");

        var token = await GenerateJwtTokenForUserAsync(user);

        return new LoginResponse
        {
            Token = token.JwtToken,
            Email = user.Email ?? string.Empty,
            RefreshToken = token.RefreshToken,
            ExpiresAt = token.ExpiresAt
        };
    }

    public async Task<RegisterResponse> RegisterUserAsync(RegisterUserRequest request)
    {

        var newUser = new ApplicationUser
        {
            UserName = request.Username,
            Email = request.Email,
            EmailConfirmed = true,
            IsOnboardingCompleted = false
        };

        var createUserResult = await _userManager.CreateAsync(newUser, request.Password);

        if (!createUserResult.Succeeded)
        {
            var errors = string.Join(", ", createUserResult.Errors.Select(e => e.Description));
            throw new Exception(
                message: "User registration failed",
                innerException: new Exception(errors)
            );
        }

        var existentRole = await _roleManager.FindByNameAsync(request.Role) ?? throw new Exception($"Role '{request.Role}' does not exist in the database.");

        var asignRoleResult = await _userManager.AddToRoleAsync(newUser, request.Role);
        if (!asignRoleResult.Succeeded)
        {
            var errors = string.Join(", ", asignRoleResult.Errors.Select(e => e.Description));
            throw new Exception(
                message: "Assigning role to user failed",
                innerException: new Exception(errors)
            );
        }

        var token = await GenerateJwtTokenForUserAsync(newUser);

        return new RegisterResponse
        {
            Email = newUser.Email,
            UserId = newUser.Id,
            UserName = newUser.UserName,
            Role = request.Role,
            RegisteredAt = DateTime.UtcNow,
            Token = token.JwtToken,
            RefreshToken = token.RefreshToken
        };
    }

    public async Task<LoginResponse> SignInWithGithubAsync(string githubEmail, string githubUsername)
    {
        var githubUser = await _userManager.FindByLoginAsync("GitHub", githubUsername);

        // If user exists, generate token and return
        if (githubUser != null)
        {
            var userToken = await GenerateJwtTokenForUserAsync(githubUser);
            return new LoginResponse
            {
                Token = userToken.JwtToken,
                Email = githubUser.Email ?? string.Empty,
                RefreshToken = userToken.RefreshToken,
                ExpiresAt = userToken.ExpiresAt
            };
        }

        var user = await _userManager.FindByEmailAsync(githubEmail);
        if (user != null)
        {
            // Link GitHub login to existing user
            var loginWithGithubResult = await _userManager.AddLoginAsync(user, new UserLoginInfo("GitHub", githubUsername, "GitHub"));
            if (!loginWithGithubResult.Succeeded)
            {
                var errors = string.Join(", ", loginWithGithubResult.Errors.Select(e => e.Description));
                throw new Exception(
                    message: "Linking GitHub login to existing user failed",
                    innerException: new Exception(errors)
                );
            }

            var userToken = await GenerateJwtTokenForUserAsync(user);

            return new LoginResponse
            {
                Token = userToken.JwtToken,
                Email = user.Email ?? string.Empty,
                RefreshToken = userToken.RefreshToken,
                ExpiresAt = userToken.ExpiresAt
            };
        }

        // If user does not exist, create new user
        var newUser = new ApplicationUser
        {
            UserName = githubUsername,
            Email = githubEmail,
            EmailConfirmed = true,
            IsOnboardingCompleted = false
        };

        var createUserResult = await _userManager.CreateAsync(newUser);
        if (!createUserResult.Succeeded)
        {
            var errors = string.Join(", ", createUserResult.Errors.Select(e => e.Description));
            throw new Exception(
                message: "GitHub user registration failed",
                innerException: new Exception(errors)
            );
        }

        // Assign Applicant role to new GitHub user

        var assignRoleResult = await _userManager.AddToRoleAsync(newUser, AppRoles.Applicant);
        if (!assignRoleResult.Succeeded)
        {
            var errors = string.Join(", ", assignRoleResult.Errors.Select(e => e.Description));
            throw new Exception(
                message: "Assigning Applicant role to GitHub user failed",
                innerException: new Exception(errors)
            );
        }

        // Register github login

        var result = await _userManager.AddLoginAsync(newUser, new UserLoginInfo("GitHub", githubUsername, "GitHub"));
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception(
                message: "Linking GitHub login to user failed",
                innerException: new Exception(errors)
            );
        }

        var token = await GenerateJwtTokenForUserAsync(newUser);

        return new LoginResponse
        {
            Token = token.JwtToken,
            Email = newUser.Email ?? string.Empty,
            RefreshToken = token.RefreshToken,
            ExpiresAt = token.ExpiresAt
        };
    }

    public async Task<GeneratedToken> RefreshTokenAsync(string refreshToken)
    {
        var principal = _tokenService.GetPrincipalFromExpiredToken(refreshToken);
        var email = principal.FindFirstValue(AppClaims.Email);

        if (email == null) throw new Exception("Invalid token.");

        var user = await _userManager.FindByEmailAsync(email) ?? throw new Exception("User not found.");
        var isValidRefreshToken = await _tokenService.IsValidRefreshTokenAsync(refreshToken, user.Id);
    
        if (!isValidRefreshToken) throw new Exception("Invalid refresh token.");

        return await GenerateJwtTokenForUserAsync(user);
    }

    private async Task<GeneratedToken> GenerateJwtTokenForUserAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Count == 0) throw new Exception("User has no roles assigned.");

        List<Claim> claims = [
            new Claim(AppClaims.UserId, user.Id),
            new Claim(AppClaims.Email, user.Email ?? string.Empty),
            new Claim(AppClaims.UserName, user.UserName ?? string.Empty),
            new Claim(AppClaims.IsOnboardingCompleted, user.IsOnboardingCompleted.ToString())
        ];

        foreach (var role in roles) claims.Add(new Claim(AppClaims.Role, role));

        return await _tokenService.GenerateJwtToken(user, claims);
    }
}