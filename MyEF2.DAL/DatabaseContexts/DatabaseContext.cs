using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyEF2.DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEF2.DAL.DatabaseContexts
{
    public class DatabaseContext : IdentityDbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }
        public DatabaseContext() { }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductTypes> ProductTypes { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Setting> Settings {get; set; }
        public DbSet<EmailLog> EmailLog { get; set; }
        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<NotificationTemplate> NotificationTemplates { get; set; }
        public DbSet<LoginHistory> LoginHistory { get; set; }
        public DbSet<StripeProduct> StripeProducts { get; set; }


        public DbSet<StripeSubscription> StripeSubscriptions { get; set; }
        public DbSet<AIDocument> AIDocuments { get; set;}
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Audit> Audits { get; set; }
        public DbSet<DeviceLoginRequest> DeviceLoginRequests { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Requirement> Requirements { get; set; }

    }
}
