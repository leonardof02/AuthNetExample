namespace Features.Companies.Models.Requests;

public record UpdateCompanyRequest(
    string? Name,
    string? Description,
    string? Email,
    string? PhoneNumber,
    string? Website
);