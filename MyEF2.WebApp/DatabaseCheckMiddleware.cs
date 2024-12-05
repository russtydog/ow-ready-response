using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MyEF2.DAL.DatabaseContexts;
using System.Threading.Tasks;

public class DatabaseCheckMiddleware
{
    private readonly RequestDelegate _next;

    public DatabaseCheckMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, DatabaseContext dbContext)
    {
        var path = context.Request.Path.ToString();
        var method = context.Request.Method;

        // Bypass the database check for specific paths
        if ((path.Equals("/CreateDatabase", StringComparison.OrdinalIgnoreCase) || 
            path.Equals("/UpdateDatabase", StringComparison.OrdinalIgnoreCase)) &&
            (method.Equals("GET", StringComparison.OrdinalIgnoreCase) || 
            method.Equals("POST", StringComparison.OrdinalIgnoreCase)))
        {
            await _next(context);
            return;
        }

        if (!await dbContext.Database.CanConnectAsync())
        {
            // Redirect to a specific Razor Page if the database cannot be connected
            context.Response.Redirect("/CreateDatabase");
            return;
        }

        // Check if all migrations have been applied
        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
        {
            // Redirect to a migration page or handle it accordingly
            context.Response.Redirect("/UpdateDatabase");
            return;
        }

        // Continue processing if the database is fully updated
        await _next(context);
    }
}