public static class MapGetCompanyById
{
    public static void ToCompany(this WebApplication app)
    {
        app.MapGet("/companies/{companyId:int}", async (int companyId, CompanyService companyService, CancellationToken cancellationToken) =>
        {
            var company = await companyService.GetCompanyByIdAsync(companyId);
            return company is not null ? Results.Ok(company) : Results.NotFound();
        })
        .WithName("GetCompanyById")
        .WithTags("Companies")
        .Produces<Company>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}