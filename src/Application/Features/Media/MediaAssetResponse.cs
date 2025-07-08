using Application.Features.Sections;
using SharedKernel;

namespace Application.Features.Media;

public sealed class MediaAssetResponse: EntityResponse {
    public string Name { get; set; }
    public string? AltName { get; set; }
    public string Url { get; set; }
    public string Type { get; set; }
    public Guid SectionId { get; set; }

    public SectionSummerResponse Section { get; set; }
}
