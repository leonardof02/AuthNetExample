using Microsoft.EntityFrameworkCore;

public class CompanyService
{
    private readonly ApplicationDbContext _dbContext;

    public CompanyService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Company> CreateCompanyAsync(CreateCompanyRequest request)
    {
        var company = new Company
        {
            Name = request.Name,
            Description = request.Description,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Website = request.Website,
            RecruiterId = request.RecruiterId
        };

        _dbContext.Companies.Add(company);
        await _dbContext.SaveChangesAsync();

        return company;
    }

    public async Task<Company?> GetCompanyByIdAsync(int companyId)
    {
        return await _dbContext.Companies.FindAsync(companyId);
    }

    public async Task<Company?> UpdateCompanyAsync(UpdateCompanyRequest company)
    {
        var existentCompany = await _dbContext.Companies.FirstOrDefaultAsync(c => c.Id == company.Id);
        if (existentCompany == null)
        {
            return null;
        }

        existentCompany.Name = company.Name;
        existentCompany.Description = company.Description;
        existentCompany.Email = company.Email;
        existentCompany.PhoneNumber = company.PhoneNumber;
        existentCompany.Website = company.Website;

        _dbContext.Companies.Update(existentCompany);
        await _dbContext.SaveChangesAsync();

        return existentCompany;
    }

    public async Task<List<Company>> GetCompaniesByRecruiterIdAsync(string recruiterId)
    {
        return await _dbContext.Companies
            .Where(c => c.RecruiterId == recruiterId)
            .ToListAsync();
    }
}