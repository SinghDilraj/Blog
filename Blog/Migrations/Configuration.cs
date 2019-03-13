namespace Blog.Migrations
{
    using Blog.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Blog.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "Blog.Models.ApplicationDbContext";
        }

        protected override void Seed(Blog.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            //role manager
            var roleManager =
                new RoleManager<IdentityRole>(
                    new RoleStore<IdentityRole>(context));

            //Admin role
            if (!context.Roles.Any(p => p.Name == "Admin"))
            {
                var adminRole = new IdentityRole("Admin");
                roleManager.Create(adminRole);
            }
            
            //Moderator role
            if (!context.Roles.Any(p => p.Name == "Moderator"))
            {
                var moderatorRole = new IdentityRole("Moderator");
                roleManager.Create(moderatorRole);
            }

            //user manager
            var userManager =
                new UserManager<ApplicationUser>(
                        new UserStore<ApplicationUser>(context));

            //Creating adminuser
            ApplicationUser admin;

            if (!context.Users.Any(
                p => p.UserName == "admin@blog.com"))
            {
                admin = new ApplicationUser();
                admin.UserName = admin.Email = "admin@blog.com";

                userManager.Create(admin, "Password-1");
            }
            else
            {
                admin = context
                    .Users
                    .First(p => p.UserName == "admin@blog.com");
            }

            //assigning admin role if not assigned
            if (!userManager.IsInRole(admin.Id, "Admin"))
            {
                userManager.AddToRole(admin.Id, "Admin");
            }
            
            //Creating adminuser
            ApplicationUser moderator;

            if (!context.Users.Any(
                p => p.UserName == "moderator@blog.com"))
            {
                moderator = new ApplicationUser();
                moderator.UserName = moderator.Email = "moderator@blog.com";

                userManager.Create(moderator, "Password-1");
            }
            else
            {
                moderator = context
                    .Users
                    .First(p => p.UserName == "moderator@blog.com");
            }

            //assigning admin role if not assigned
            if (!userManager.IsInRole(moderator.Id, "moderator"))
            {
                userManager.AddToRole(moderator.Id, "moderator");
            }
        }
    }
}
