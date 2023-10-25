namespace NotionBackend.Api.Infrastructure.Startup;

public static partial class EndpointMapper
{
    public static WebApplication RegisterEndpoints(this WebApplication app)
    {
        app.MapControllers();
        app.MapGet("/api/health-check", () => "API is ON!");
        app.MapControllerRoute(
            "default",
            "{controller=Home}/{action=Index}/{id?}");

        return app;
    }
}
