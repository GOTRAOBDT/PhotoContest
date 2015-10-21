namespace PhotoContest.App.Models.Contest
{
    using System;
    using System.Linq.Expressions;

    using PhotoContest.Models;

    public class SummaryContestViewModel
    {
        public static Expression<Func<Contest, SummaryContestViewModel>> Create
        {
            get
            {
                return c => new SummaryContestViewModel
                {
                    Id = c.Id,
                    Title = c.Title,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    Thumbnail = c.Thumbnail,
                    Owner = c.Owner.UserName,
                    Status = c.Status.ToString(),
                    VotingType = c.VotingType.ToString(),
                    ParticipationType = c.ParticipationType.ToString(),
                    DeadlineType = c.DeadlineType.ToString(),
                    PicturesCount = c.Pictures.Count,
                    ParticipantsCount = c.Participants.Count,
                    VotesCount = c.Votes.Count
                };
            }
        }

        public int Id { get; set; }

        public string Title { get; set; }
        
        public DateTime StartDate { get; set; }
        
        public DateTime EndDate { get; set; }

        public string Thumbnail { get; set; }

        public string Owner { get; set; }

        public bool IsOwner { get; set; }
        
        public string Status { get; set; }
        
        public string VotingType { get; set; }

        public string ParticipationType { get; set; }
        
        public string DeadlineType { get; set; }

        public int PicturesCount { get; set; }

        public int ParticipantsCount { get; set; }

        public int VotesCount { get; set; }
    }
}