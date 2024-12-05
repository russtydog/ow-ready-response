using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyEF2.DAL.DatabaseContexts;
using System;

namespace MyEF2.WebApp.Pages
{
    public class UpdateDatabaseModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        public UpdateDatabaseModel(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var connectionString = _configuration.GetConnectionString("Default");

            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
            optionsBuilder.UseSqlServer(connectionString);

            using (var dbContext = new DatabaseContext(optionsBuilder.Options))
            {
                var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
                if (!pendingMigrations.Any()){
                    return RedirectToPage("/Dashboard");
                }
        
            }

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var connectionString = _configuration.GetConnectionString("Default");

            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
            optionsBuilder.UseSqlServer(connectionString);

            using (var dbContext = new DatabaseContext(optionsBuilder.Options))
            {
                await dbContext.Database.MigrateAsync();
            }

            return RedirectToPage("/Dashboard");
        }
    }
}