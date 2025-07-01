using SharedKernel;

namespace Domain.Sections;

public static class SectionErrors
{
    public static Error NotFound(Guid projectId) => Error.NotFound(
        "Sections.NotFound",
        $"The section with the Id = '{projectId}' was not found");
    public static readonly Error AllowedUsersNotFound = Error.Validation(
        "Section.AllowedUsersNotFound",
        "One of the allowed users was not found.");
}
