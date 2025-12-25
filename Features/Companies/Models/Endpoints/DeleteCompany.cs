public static class MapDeleteCompany
{
    public static void MapDeleteCompanyEndpoint(this WebApplication app)
    {
        app.MapDelete("/companies/{companyId:int}", async (int companyId, CompanyService companyService, CancellationToken cancellationToken) =>
        {
            var deleted = await companyService.DeleteCompanyAsync(companyId);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteCompany")
        .WithTags("Companies")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}