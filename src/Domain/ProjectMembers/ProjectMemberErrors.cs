using SharedKernel;

namespace Domain.ProjectMembers;

public static class ProjectMemberErrors
{
    public static Error AlreadyExists(Guid projectId, Guid userId) => Error.Conflict(
        "ProjectMembers.AlreadyExists",
        $"User '{userId}' is already a member of project '{projectId}'.");
    
    public static Error NotAMember(Guid projectId, Guid userId) => Error.NotFound(
        "ProjectMembers.NotFound",
        $"User '{userId}' is not a member of project '{projectId}'.");
    
    public static Error OwnerCannotLeave(Guid projectId) => Error.Conflict(
        "ProjectMembers.OwnerCannotLeave",
        $"The owner cannot leave their own project '{projectId}'.");
    
    public static Error CannotRemoveOwner(Guid projectId) => Error.Conflict(
        "ProjectMembers.CannotRemoveOwner",
        $"You cannot remove the owner from project '{projectId}'.");
    
    public static Error CannotModifyOwner(Guid projectId) => Error.Conflict(
        "ProjectMembers.CannotModifyOwner",
        $"You cannot modify the role of the project owner for project '{projectId}'.");
}
