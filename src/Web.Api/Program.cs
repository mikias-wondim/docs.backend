using System.Reflection;
using Application;
using HealthChecks.UI.Client;
using Infrastructure;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using Web.Api;
using Web.Api.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// ---------------------------
// Configure Logging
// ---------------------------
builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

// ---------------------------
// Register Services
// ---------------------------
builder.Services.AddSwaggerGenWithAuth();

builder.Services
    .AddApplication()
    .AddPresentation()
    .AddInfrastructure(builder.Configuration);

// Allow all origins for development
builder.Services.AddCors(options => options.AddPolicy("DevCorsPolicy",
        policy => policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()));

builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

// ---------------------------
// Build App
// ---------------------------
WebApplication app = builder.Build();

// ---------------------------
// Middleware Pipeline
// ---------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerWithUi();
    // await app.ApplyMigrations();
}

app.UseCors("DevCorsPolicy");

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseRequestContextLogging();
app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.UseUploadStaticFile();

app.MapEndpoints();

// For controller-based endpoints
app.MapControllers();

await app.RunAsync();

// Required for functional/integration tests
namespace Web.Api
{
    public partial class Program;
}
