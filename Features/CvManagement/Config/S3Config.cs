public record S3Config
{
    public required string Profile { get; set; }
    public required string Region { get; set; }
    public required string ServiceURL { get; set; }
    public bool ForcePathStyle { get; set; } = true;
    public bool UseArnRegion { get; set; } = false;
    public required string AccessKey { get; set; }
    public required string SecretKey { get; set; }
}