namespace PhotoContest.Console
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using PhotoContest.Data;
    using PhotoContest.Models;
    using PhotoContest.Models.Enumerations;

    /// <summary>
    /// This app is used entirely for the purpose of testing the db model and db context.
    /// Uncomment lines one by one to conduct your own tests.
    /// </summary>
    public class PhotoContestConsole
    {
        private static PhotoContestContext dbContext = new PhotoContestContext();

        public static void Main()
        {
            AddSomeUsers();
            Console.WriteLine(dbContext.Users.Count());

            CreateContest();
            Console.WriteLine(dbContext.Contests.Count());

            AddPicture();
            Console.WriteLine(dbContext.Pictures.Count());

            var contest = dbContext.Contests.First();
            var picture = dbContext.Pictures.First();
            var participant = dbContext.Users.First();
            var voter = dbContext.Users.Where(u => u.UserName == "asya").FirstOrDefault();

            contest.Participants.Add(participant);
            contest.Pictures.Add(picture);
            var vote = new Vote()
            {
                Contest = contest,
                Voter = voter,
                Picture = picture
            };

            contest.Votes.Add(vote);
            dbContext.SaveChanges();

            Console.WriteLine(contest.Pictures.Count());
            Console.WriteLine(contest.Votes.Count());
        }

        private static void AddPicture()
        {
            var author = dbContext.Users.First();
            var picture = new Picture()
            {
                Author = author,
                PictureData = "vse endo e base64 string",
                PostedOn = DateTime.Now
            };
            dbContext.Pictures.Add(picture);
            dbContext.SaveChanges();
        }

        private static void CreateContest()
        {
            var owner = dbContext.Users.First();
            var contest = new Contest()
            {
                Title = "Some contest",
                Description = "Aide na konkursite!",
                Status = ContestStatus.Active,
                Owner = owner,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                DeadlineType = DeadlineType.ParticipationLimit,
                VotingType = VotingType.Open,
                ParticipationType = ParticipationType.Open
            };
            dbContext.Contests.Add(contest);
            dbContext.SaveChanges();
        }

        public static void AddSomeUsers()
        {
            var user1 = new User()
            {
                UserName = "straho",
                Name = "Strahil1",
                Gender = UserGender.Male,
                BirthDate = 40
            };

            var user2 = new User()
            {
                UserName = "anatoli",
                Name = "Anatoli",
                Gender = UserGender.Male,
                BirthDate = 21
            };

            var user3 = new User()
            {
                UserName = "asya",
                Name = "Asya",
                Gender = UserGender.Female,
                BirthDate = 18
            };

            var user4 = new User()
            {
                UserName = "barish",
                Name = "Barish",
                Gender = UserGender.Male,
                BirthDate = 21
            };

            dbContext.Users.Add(user1);
            dbContext.Users.Add(user2);
            dbContext.Users.Add(user3);
            dbContext.Users.Add(user4);
            dbContext.SaveChanges();
        }
    }
}
