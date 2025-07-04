using Application.Abstractions.Data;
using Domain.Auth;
using Domain.Faqs;
using Domain.Invitations;
using Domain.Pages;
using Domain.ProjectMembers;
using Domain.Projects;
using Domain.Sections;
using Domain.Users;
using Infrastructure.DomainEvents;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Infrastructure.Database;

public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    IDomainEventsDispatcher domainEventsDispatcher)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectMember> ProjectMembers { get; set; }
    public DbSet<Invitation> Invitations { get; set; }
    public DbSet<Section> Sections { get; set; }
    public DbSet<SectionUserAccess> SectionUserAccesses { get; set; }
    public DbSet<Page> Pages { get; set; }
    public DbSet<Faq> Faqs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        modelBuilder.HasDefaultSchema(Schemas.Default);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        int result = await base.SaveChangesAsync(cancellationToken);

        await PublishDomainEventsAsync();

        return result;
    }

    private async Task PublishDomainEventsAsync()
    {
        var domainEventEntities = ChangeTracker
            .Entries<Entity>()
            .Where(entry => entry.Entity.DomainEvents.Any())
            .Select(entry => entry.Entity)
            .ToList();

        var allDomainEvents = domainEventEntities
            .SelectMany(entity => entity.DomainEvents)
            .ToList();

        // Clear events after collecting them to avoid mutation during dispatch
        domainEventEntities.ForEach(entity => entity.ClearDomainEvents());

        await domainEventsDispatcher.DispatchAsync(allDomainEvents);
    }
}
