using SharedKernel;

namespace Domain.Pages;

public static class PageErrors
{
    public static Error NotFound(Guid pageId) =>
        Error.NotFound(
            "Pages.NotFound",
            $"The page with ID '{pageId}' was not found.");

    public static Error Forbidden =>
        Error.Forbidden(
            "Pages.Forbidden",
            "You do not have permission to access this page.");
}
