using Microsoft.Extensions.FileProviders;

namespace Web.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseSwaggerWithUi(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        
        return app;
    }
    
    public static IApplicationBuilder UseUploadStaticFile(this WebApplication app)
    {
        string uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(uploadsPath),
            RequestPath = "/uploads"
        });
        
        return app;
    }
}
