﻿using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BlogWebsite.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("BlogApplicationContext", throwIfV1Schema: false)
        {
            Database.SetInitializer<ApplicationDbContext>(new DropCreateDatabaseIfModelChanges<ApplicationDbContext>());
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<BlogWebsite.Models.Post> Posts { get; set; }
        public System.Data.Entity.DbSet<BlogWebsite.Models.Tag> Tags { get; set; }
        public System.Data.Entity.DbSet<BlogWebsite.Models.Comment> Comments { get; set; }
        public System.Data.Entity.DbSet<BlogWebsite.Models.ImageModel> Image { get; set; }
        public System.Data.Entity.DbSet<BlogWebsite.Models.Followers> Followers { get; set; }
        public System.Data.Entity.DbSet<BlogWebsite.Models.Author> Authors { get; set; }
        public System.Data.Entity.DbSet<BlogWebsite.Models.Like> Like { get; set; }
        public System.Data.Entity.DbSet<BlogWebsite.Models.Notifications> Notifications { get; set; }
    }
}