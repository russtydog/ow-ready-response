using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using MyEF2.DAL.DatabaseContexts;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Services;
using MyEF2.WebApp;
using Stripe;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddDbContext<DatabaseContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));


builder.Services.AddHttpsRedirection(options =>
        {
            options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
            // options.HttpsPort = 7253; // Ensure this is the correct port for your HTTPS
        });

builder.Services.AddScoped<MyEF2.DAL.Services.ProductService>();
builder.Services.AddScoped<ProductTypeService>();
builder.Services.AddScoped<StatusService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<SettingService>();
builder.Services.AddScoped<EmailLogService>();
builder.Services.AddScoped<OrganisationService>();
builder.Services.AddScoped<NotificationTemplateService>();
builder.Services.AddScoped<LoginHistoryService>();
builder.Services.AddScoped<StripeProductService>();
builder.Services.AddScoped<StripeSubscriptionService>();
builder.Services.AddScoped<ConversationService>();
builder.Services.AddScoped<MessageService>();
builder.Services.AddScoped<MenuService>();
builder.Services.AddScoped<AuditService>();
builder.Services.AddScoped<DeviceLoginRequestService>();
builder.Services.AddScoped<ArticleService>();
builder.Services.AddScoped<RequirementService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});



builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
}
).AddEntityFrameworkStores<DatabaseContext>()
.AddDefaultTokenProviders()
.AddTokenProvider<MyEF2TokenProvider>("AppTokenProvider");


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();
app.UseMiddleware<DatabaseCheckMiddleware>();

app.UseAuthentication();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();
app.UseCors("AllowAll");

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
});

app.Run();
