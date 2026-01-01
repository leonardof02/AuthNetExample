using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public static class UploadCvEndpoint
{
    public static void MapUploadCvEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/profile/applicant/cv", async (IFormFile file, [FromServices] CvStorageService cvStorageService) =>
        {
            if (file == null || file.Length == 0)
            {
                return Results.BadRequest("No file uploaded.");
            }

            using var stream = file.OpenReadStream();
            var fileUrl = await cvStorageService.UploadCvAsync(file.FileName, stream);

            return Results.Ok(new { Url = fileUrl });
        })
        .RequireAuthorization(new AuthorizeAttribute
        {
            Roles = AppRoles.Applicant,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
        })
        .DisableAntiforgery();
    }
}