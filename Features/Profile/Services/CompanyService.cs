using System.Security.Claims;
using AuthNetExample.Features.Auth.Models.Constants;
using Features.Companies.Models.Requests;

public class CompanyService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CompanyService(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Company> CreateCompanyAsync(CreateCompanyRequest request)
    {
        var recruiterId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ??
            throw new Exception("User is not authenticated.");

        var company = new Company
        {
            RecruiterId = recruiterId,
            Name = request.Name,
            Description = request.Description,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Website = request.Website
        };

        _dbContext.Companies.Add(company);
        await _dbContext.SaveChangesAsync();
        return company;
    }

    public async Task UpdateCompanyAsync(int companyId, UpdateCompanyRequest request)
    {
        var company = await _dbContext.Companies.FindAsync(companyId)
            ?? throw new Exception("Company not found.");

        var recruiterId = (_httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier))
            ?? throw new Exception("User is not authenticated.");

        if (company.RecruiterId != recruiterId)
            throw new Exception("User is not authorized to update this company.");

        company.Name = request.Name ?? company.Name;
        company.Description = request.Description ?? company.Description;
        company.Email = request.Email ?? company.Email;
        company.PhoneNumber = request.PhoneNumber ?? company.PhoneNumber;
        company.Website = request.Website ?? company.Website;

        await _dbContext.SaveChangesAsync();
    }
}