using Application.Abstractions.Messaging;
using Domain.ProjectMembers;
using Domain.Sections;

namespace Application.Features.Sections.Create;

public sealed record CreateSectionCommand(
    Guid ProjectId,
    string Name,
    string? Description,
    SectionVisibility Visibility,
    string? Password,
    List<ProjectRole>? AllowedRoles,
    List<Guid>? AllowedUserIds,
    int Order
) : ICommand<Guid>;
