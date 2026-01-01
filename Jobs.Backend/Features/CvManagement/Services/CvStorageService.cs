using System.Security.Claims;
using Amazon.S3;
using Amazon.S3.Model;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

public class CvStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ApplicationDbContext _dbContext;

    private const string BucketName = "cvs";

    public CvStorageService(
        BlobServiceClient blobServiceClient,
        IHttpContextAccessor httpContextAccessor,
        ApplicationDbContext dbContext
    )
    {
        _blobServiceClient = blobServiceClient;
        _httpContextAccessor = httpContextAccessor;
        _dbContext = dbContext;
    }

    public async Task<string> UploadCvAsync(string fileName, Stream fileStream)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(BucketName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

        var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        var role = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);

        if (userId == null) throw new UnauthorizedAccessException("User is not authenticated.");
        if (role != AppRoles.Applicant) throw new UnauthorizedAccessException("Only applicants can upload CVs.");

        string objectKey = $"{userId}/{fileName}";

        var blobClient = containerClient.GetBlobClient(objectKey);

        var cv = new Cv
        {
            Name = fileName,
            Url = blobClient.Uri.ToString(),
            UserId = userId
        };

        using (var stream = fileStream)
        {
            await blobClient.UploadAsync(stream, overwrite: true);
        }

        _dbContext.Set<Cv>().Add(cv);
        await _dbContext.SaveChangesAsync();

        return cv.Url;
    }
}