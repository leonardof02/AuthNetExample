using Jobs.Backend.Features.Auth.Models.Constants;
using Microsoft.AspNetCore.Authorization;

public static class AddFirstTimeUsingTheAppPolicy
{
    public const string PolicyName = "FirstTimeUsingTheApp";

    public static AuthorizationOptions AddFirstTimeUsingTheAppPolicyService(this AuthorizationOptions options)
    {

        options.AddPolicy(PolicyName, policy =>
        {
            policy.RequireClaim(AppClaims.IsOnboardingCompleted, false.ToString());
        });


        return options;
    }
}