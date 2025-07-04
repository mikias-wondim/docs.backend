using SharedKernel;

namespace Domain.Faqs;

public static class FaqErrors
{
    public static Error NotFound(Guid pageId) =>
        Error.NotFound(
            "Faqs.NotFound",
            $"The faq with ID '{pageId}' was not found.");

    public static Error Forbidden =>
        Error.Forbidden(
            "Faqs.Forbidden",
            "You do not have permission to access this faq.");
}
