using SharedKernel;

namespace Domain.Projects;

public static class ProjectErrors
{
    public static Error NotFound(Guid todoItemId) => Error.NotFound(
        "Projects.NotFound",
        $"The project with the Id = '{todoItemId}' was not found");
}
