namespace PhotoContest.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;

    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    using Models;
    using Models.Enumerations;

    internal sealed class Configuration : DbMigrationsConfiguration<PhotoContestContext>
    {
        public Configuration()
        {
            this.AutomaticMigrationsEnabled = true;
            this.AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(PhotoContestContext context)
        {
            if (context.Users.Count() > 1)
            {
                return;
            }

            CreateAdmin(context);
            CreateUser(context, "strahil", "123");
            CreateUser(context, "barish", "123");
            CreateUser(context, "anatoli", "123");
            CreateUser(context, "asya", "123");
        }

        private void CreateUser(PhotoContestContext context, string username, string password)
        {
            var userManager = new UserManager<User>(new UserStore<User>(context));

            var user = new User()
            {
                UserName = username,
                Name = username.ToUpper(),
                Gender = UserGender.Other,
                Email = username + "@gmail.com",
            };

            var result = userManager.Create(user, username + password);
            context.SaveChanges();
        }

        private void CreateAdmin(PhotoContestContext context)
        {
            var userManager = new UserManager<User>(new UserStore<User>(context));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            string roleName = "Administrator";
            IdentityResult roleResult;

            if (!roleManager.RoleExists(roleName))
            {
                roleResult = roleManager.Create(new IdentityRole(roleName));
            }

            var adminUser = new User()
            {
                UserName = "Oneadmintorulethemall",
                Name = "AndIntoDarknessBindThem",
                Gender = Models.Enumerations.UserGender.Other,
                Email = "TheDarkLord@admin.com",
            };

            var result = userManager.Create(adminUser, "Admin123");
            if (result.Succeeded)
            {
                var currentUser = userManager.FindByName(adminUser.UserName);
                var roleresult = userManager.AddToRole(currentUser.Id, "Administrator");
            }
            context.SaveChanges();
        }
    }
}
