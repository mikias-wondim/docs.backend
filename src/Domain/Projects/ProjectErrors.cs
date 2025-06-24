using SharedKernel;

namespace Domain.Projects;

public static class ProjectErrors
{
    public static Error NotFound(Guid projectId) => Error.NotFound(
        "Projects.NotFound",
        $"The project with the Id = '{projectId}' was not found");
    public static Error Unauthorized(Guid projectId) => Error.Unauthorized(
        "Projects.Unauthorized",
        $"The project with the Id = '{projectId}' is not owned by the current user.");
}
