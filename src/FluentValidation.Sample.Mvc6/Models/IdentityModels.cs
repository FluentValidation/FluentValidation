using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.SqlServer;
using Microsoft.Data.Entity;
using Microsoft.Framework.OptionsModel;

namespace FluentValidation.Sample.Mvc6.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {

    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private static bool _created = false;
        
        public ApplicationDbContext(IServiceProvider serviceProvider, IOptionsAccessor<DbContextOptions> optionsAccessor)
            : base(serviceProvider, optionsAccessor.Options)
        {            
            // Create the database and schema if it doesn't exist
            // This is a temporary workaround to create database until Entity Framework database migrations 
            // are supported in ASP.NET vNext
            if (!_created)
            {
                Database.EnsureCreated();
                _created = true;
            }
        }
    }
}