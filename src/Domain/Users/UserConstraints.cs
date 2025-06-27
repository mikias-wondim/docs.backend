namespace Domain.Users;

public static class UserConstraints
{
    public const int MaxEmailLength = 200;
    public const int MaxFirstNameLength = 100;
    public const int MaxLastNameLength = 100;
    public const int MaxDisplayNameLength = 100;
    public const int MaxBioLength = 1000;
    public const int MaxAvatarUrlLength = 2048;
    
    public static readonly string[] AllowedFileTypes =
    [
        "image/jpeg",
        "image/png",
        "image/jpg",
        "image/webp"
    ];

    public const int MaxFileSizeInBytes = 4 * 1024 * 1024;
}
