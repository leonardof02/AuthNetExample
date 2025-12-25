public static class MapCreateCompany
{
    public static void MapCreateCompanyEndpoint(this WebApplication app)
    {
        app.MapPost("/companies", async (CreateCompanyRequest request, CompanyService companyService, CancellationToken cancellationToken) =>
        {
            var company = await companyService.CreateCompanyAsync(request);
            return Results.Created($"/companies/{company.Id}", company);
        })
        .AddEndpointFilter<ValidationFilter<CreateCompanyRequest>>()
        .WithName("CreateCompany")
        .WithTags("Companies")
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}