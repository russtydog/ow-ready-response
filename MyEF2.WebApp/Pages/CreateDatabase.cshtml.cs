using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MyEF2.DAL.DatabaseContexts;
using System.Net;
using System.Text.Encodings.Web;

namespace MyEF2.WebApp.Pages
{
    
    public class CreateDatabaseModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        public CreateDatabaseModel(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }
        [BindProperty]
        public string Server{get;set;}
        [BindProperty]
        public string Database{get;set;}
        [BindProperty]
        public string SAUsername{get;set;}
        [BindProperty]
        public string SAPassword{get;set;}
        [BindProperty]
        public string Username{get;set;}
        [BindProperty]
        public string Password{get;set;}
        public string ConnectionString { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            //check if the default DatabaseContext database actually exists or not
            //create a local instance of DatabaseContext
            try{
                var connectionString = _configuration.GetConnectionString("Default");
                ConnectionString = _configuration.GetConnectionString("Default");

                var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
                optionsBuilder.UseSqlServer(connectionString);

                using (var dbContext = new DatabaseContext(optionsBuilder.Options))
                {
                    if(!await dbContext.Database.CanConnectAsync()){
                        return Page();
                    }
                    else{
                        return RedirectToPage("/Dashboard");
                     }
                }
            }
            catch(Exception ex){
                //errored, however may have worked so just go back home
                return RedirectToPage("/Dashboard");

            }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            //do all the fancy things to create stuff
            string masterConnectionString = "Server=" + Server + ";Database=master;User Id="+ SAUsername+";Password="+SAPassword+";TrustServerCertificate=true";
            string dbConnectionString = $"Server="+Server+";Database="+Database+";User Id="+SAUsername+";Password="+SAPassword +";TrustServerCertificate=true";

            try
            {
                using (var connection = new SqlConnection(masterConnectionString))
                {
                    connection.Open();

                    // Create the database
                    try{
                    var createDbCommand = new SqlCommand("CREATE DATABASE [" + Database + "]", connection);
                    createDbCommand.ExecuteNonQuery();
                    }
                    catch(Exception ex){
                        Console.WriteLine("Error Creating Database.");
                        Console.WriteLine(ex.Message);
                    }

                    // Create the new user
                    try{
                        using (var dbConnection = new SqlConnection(dbConnectionString))
                        {
                            dbConnection.Open();
                            var createUserCommand = new SqlCommand(@"
                                CREATE LOGIN "+ Username + @" WITH PASSWORD = '" + Password + @"';
                                USE [" + Database + @"];
                                CREATE USER " + Username + @" FOR LOGIN " + Username + @";
                                EXEC sp_addrolemember 'db_datareader', " + Username + @";
                                EXEC sp_addrolemember 'db_datawriter', " + Username + @";
                                EXEC sp_addrolemember 'db_owner', " + Username + @";
                                
                            ", dbConnection);
                            createUserCommand.ExecuteNonQuery();
                        }
                    }
                    catch(Exception ex){
                        Console.WriteLine("Error Creating Users.");
                        Console.WriteLine(ex.Message);
                        return RedirectToPage("/Dashboard");
                    }
                }

                return RedirectToPage("/Dashboard");
            }
            catch (Exception ex)
            {
                // Log and handle the exception
                ModelState.AddModelError("", "An error occurred while setting up the database: " + ex.Message);
                return Page();
            }

            return RedirectToPage("/Dashboard");
        }
    }
    
}