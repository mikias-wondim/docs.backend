using SharedKernel;

namespace Domain.Media;

public static class MediaAssetErrors
{
    public static Error EmptyFile => Error.Validation(
        "MediaAsset.EmptyFile", 
        "The uploaded file is empty.");

    public static Error NotFound(string path) => Error.NotFound(
            "MediaAsset.NotFound", 
            $"The file at path '{path}' was not found.");

    public static Error InvalidFormat(string allowedFormats) => Error.Validation(
        "MediaAsset.InvalidFormat", 
        $"The uploaded file format is invalid. Allowed formats: {allowedFormats}.");

    public static Error AccessDenied => Error.Forbidden(
        "MediaAsset.AccessDenied", 
        "You do not have permission to access this media asset.");

    public static Error UploadFailed => Error.Problem(
        "MediaAsset.UploadFailed", 
        "An error occurred while uploading the media asset.");

    public static Error DeleteFailed => Error.Problem(
        "MediaAsset.DeleteFailed", 
        "An error occurred while deleting the media asset.");
}
