using Application.Abstractions.Messaging;

namespace Application.Features.Projects.TransferOwnership;

public record TransferProjectOwnershipCommand(Guid ProjectId, Guid NewOwnerId) : ICommand;
