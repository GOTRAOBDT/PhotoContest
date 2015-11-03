namespace PhotoContest.Data.Migrations
{
    using System;
    using System.Collections.Generic;
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

            this.CreateAdmin(context);
            this.CreateUserWithContests(context, "strahil", "123");
            this.CreateUserWithContests(context, "barish", "123");
            this.CreateUserWithContests(context, "anatoli", "123");
            this.CreateUserWithContests(context, "asya", "123");
        }

        private void CreateUserWithContests(PhotoContestContext context, string username, string password)
        {
            Random rnd = new Random();
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

            //var contests = new List<Contest>
            //{
            //    new Contest
            //    {
            //        Title = "Nature",
            //        Description = "Photos of nature",
            //        StartDate = DateTime.Now.AddDays(rnd.Next(-100, -15)),
            //        EndDate = DateTime.Now.AddDays(rnd.Next(15, 50)),
            //        OwnerId = user.Id,
            //        Status = ContestStatus.Active,
            //        VotingType = VotingType.Open,
            //        ParticipationType = ParticipationType.Open,
            //        DeadlineType = DeadlineType.ParticipationLimit,
            //    },
            //    new Contest
            //    {
            //        Title = "Portrets",
            //        Description = "Portrets photos",
            //        StartDate = DateTime.Now.AddDays(rnd.Next(-100, -50)),
            //        EndDate = DateTime.Now.AddDays(rnd.Next(-5, -1)),
            //        OwnerId = user.Id,
            //        Status = ContestStatus.Finished,
            //        VotingType = VotingType.Closed,
            //        ParticipationType = ParticipationType.Open,
            //        DeadlineType = DeadlineType.EndDate,
            //    },
            //    new Contest
            //    {
            //        Title = "Street 111",
            //        Description = "Street photography",
            //        StartDate = DateTime.Now.AddDays(rnd.Next(15, 50)),
            //        EndDate = DateTime.Now.AddDays(rnd.Next(100, 150)),
            //        OwnerId = user.Id,
            //        Status = ContestStatus.Inactive,
            //        VotingType = VotingType.Open,
            //        ParticipationType = ParticipationType.Open,
            //        DeadlineType = DeadlineType.EndDate,
            //    },
            //    new Contest
            //    {
            //        Title = "Street 222",
            //        Description = "Street photography",
            //        StartDate = DateTime.Now.AddDays(rnd.Next(15, 50)),
            //        EndDate = DateTime.Now.AddDays(rnd.Next(100, 150)),
            //        OwnerId = user.Id,
            //        Status = ContestStatus.Inactive,
            //        VotingType = VotingType.Open,
            //        ParticipationType = ParticipationType.Open,
            //        DeadlineType = DeadlineType.EndDate,
            //    },
            //    new Contest
            //    {
            //        Title = "Portrets at sunset",
            //        Description = "Portrets at sunset photos",
            //        StartDate = DateTime.Now.AddDays(rnd.Next(-100, -15)),
            //        EndDate = DateTime.Now.AddDays(rnd.Next(15, 50)),
            //        OwnerId = "1",
            //        Status = ContestStatus.Active,
            //        VotingType = VotingType.Closed,
            //        ParticipationType = ParticipationType.Open,
            //        DeadlineType = DeadlineType.EndDate,
            //    },
            //};

            //foreach (var contest in contests)
            //{
            //    context.Contests.Add(contest);
            //    user.MyContests.Add(contest);
            //}

            //context.SaveChanges();
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
