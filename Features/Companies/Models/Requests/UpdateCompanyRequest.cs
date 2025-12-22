public record UpdateCompanyRequest(
    int Id,
    string Name,
    string? Description,
    string? Email,
    string? PhoneNumber,
    string? Website
);