using SharedKernel;

namespace Domain.Invitations;

public static class InvitationErrors
{
    public static Error NotFound(string id) => Error.NotFound(
        "Invitation.NotFound",
        $"The invitation with ID '{id}' was not found.");
    
    public static Error FailedToSendEmail => Error.Problem(
        "Invitation.FailedToSendEmail",
        "Failed to send invitation email to the user.");

    public static Error AlreadyHandled => Error.Conflict(
        "Invitation.AlreadyHandled",
        "This invitation has already been accepted or rejected.");
    
    public static readonly Error Expired = Error.Conflict(
        "Invitations.Expired",
        "This invitation has expired.");
    
    public static Error DuplicateInvitation => Error.Conflict(
        "Invitation.Duplicate",
        "An invitation for this user and project already exists.");
}
