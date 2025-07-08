using Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Media.Upload;

public sealed record UploadSectionAssetCommand (Guid SectionId, IFormFile File, string? AltName): ICommand<MediaAssetResponse>;
