using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Web.Api.Extensions;

public static class MigrationExtensions
{
    public static async Task ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        await using ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        ILogger<ApplicationDbContext> logger = 
            scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
        
        try
        {
            await dbContext.Database.MigrateAsync();
            logger.LogInformation("Database migrated successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database");

            throw new ApplicationException("Failed during database migration in ApplyMigrations extension.", ex);
        }
    }
}
