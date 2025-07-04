using Application.Features.Pages;
using Application.Features.Projects;
using Domain.ProjectMembers;
using Domain.Sections;
using SharedKernel;

namespace Application.Features.Sections;

public sealed class SectionResponse: EntityResponse
{
    public Guid ProjectId { get; init; }
    public Guid DefaultPageId { get; set; }
    
    public string Name { get; init; }
    public string? Description { get; init; }
    public int Order { get; init; }

    public SectionVisibility Visibility { get; init; }
    public string? Password { get; init; }
    public List<ProjectRole>? AllowedRoles { get; init; }
    public List<SectionUserAccessResponse> AllowedUsers { get; init; } = [];
    
    public ProjectSummaryResponse Project { get; init; }
    public List<PageResponse> Pages { get; set; } = []; 
}


