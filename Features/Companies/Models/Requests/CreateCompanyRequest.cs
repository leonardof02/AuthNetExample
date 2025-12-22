public record CreateCompanyRequest(
    string Name,
    string? Description,
    string? Email,
    string? PhoneNumber,
    string? Website,
    string RecruiterId
);