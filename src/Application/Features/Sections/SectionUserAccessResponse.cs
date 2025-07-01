using Application.Features.Users;
using SharedKernel;

namespace Application.Features.Sections;

public sealed class SectionUserAccessResponse: EntityResponse
{
    public Guid SectionId { get; set; }
    public Guid UserId { get; set; }

    public UserSummaryResponse User { get; set; } = null!;
}
