namespace PhotoContest.App.Models.Contest
{
    using System;

    using AutoMapper;

    using Bookmarks.Common.Mappings;

    using PhotoContest.Common;
    using PhotoContest.Models;

    public class SummaryContestViewModel : IMapFrom<Contest>, IHaveCustomMappings
    {
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

        public int? ParticipationLimit { get; set; }

        public int PicturesCount { get; set; }

        public int ParticipantsCount { get; set; }

        public int VotesCount { get; set; }

        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<Contest, SummaryContestViewModel>()
                .ForMember(c => c.Owner, cnf => cnf.MapFrom(m => m.Owner.UserName))
                .ForMember(c => c.Status, cnf => cnf.MapFrom(m => m.Status.ToString()))
                .ForMember(c => c.VotingType, cnf => cnf.MapFrom(m => m.VotingType.ToString()))
                .ForMember(c => c.ParticipationType, cnf => cnf.MapFrom(m => m.ParticipationType.ToString()))
                .ForMember(c => c.DeadlineType, cnf => cnf.MapFrom(m => m.DeadlineType.ToString()))
                .ForMember(c => c.ParticipantsCount, cnf => cnf.MapFrom(m => m.Pictures.Count))
                .ForMember(c => c.ParticipantsCount, cnf => cnf.MapFrom(m => m.Participants.Count))
                .ForMember(c => c.VotesCount, cnf => cnf.MapFrom(m => m.Votes.Count))
                .ForMember(c => c.Thumbnail, cnf => cnf.MapFrom(m => m.Thumbnail ?? GlobalConstants.DefaultContestThumbnail));
        }
    }
}