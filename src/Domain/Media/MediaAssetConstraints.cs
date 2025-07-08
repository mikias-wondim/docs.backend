namespace Domain.Media;

public static class MediaAssetConstraints
{
    public static readonly string[] AllowedContentTypes =
    [
        "image/jpeg", "image/jpg", "image/png", "image/webp", "image/svg+xml", "image/gif"
    ];

    public const long MaxFileSizeInBytes = 5 * 1024 * 1024;  
    public const int MaxNameLength = 200;  
    public const int MaxAltNameLength = 200;  
    public const int MaxUrlLength = 1000;  
    public const int MaxTypeLength = 50;  
}
