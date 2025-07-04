using Domain.Auth;
using Domain.Invitations;
using Domain.Pages;
using Domain.ProjectMembers;
using Domain.Projects;
using Domain.Sections;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Data;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<EmailVerificationToken> EmailVerificationTokens { get; }
    DbSet<Project> Projects { get; }
    DbSet<ProjectMember> ProjectMembers { get; }
    DbSet<Invitation> Invitations { get; }
    DbSet<Section> Sections { get; }
    DbSet<SectionUserAccess> SectionUserAccesses { get; }
    DbSet<Page> Pages { get; }


    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
