public static class MapUpdateCompany
{
    public static void MapUpdateCompanyEndpoint(this WebApplication app)
    {
        app.MapPut("/companies/{companyId:int}", async (int companyId, UpdateCompanyRequest request, CompanyService companyService, CancellationToken cancellationToken) =>
        {
            if (companyId != request.Id)
            {
                return Results.BadRequest("Company ID in the URL does not match the ID in the request body.");
            }

            var updatedCompany = await companyService.UpdateCompanyAsync(request);
            return updatedCompany is not null ? Results.Ok(updatedCompany) : Results.NotFound();
        })
        .AddEndpointFilter<ValidationFilter<UpdateCompanyRequest>>()
        .WithName("UpdateCompany")
        .WithTags("Companies")
        .Produces<Company>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}