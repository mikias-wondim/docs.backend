using Application.Abstractions.Messaging;
using Domain.ProjectMembers;
using Domain.Sections;

namespace Application.Features.Sections.Update;

public sealed record UpdateSectionCommand(
    Guid SectionId,
    string Name,
    string? Description,
    SectionVisibility Visibility,
    string? Password,
    List<ProjectRole>? AllowedRoles,
    List<Guid>? AllowedUserIds): ICommand;
