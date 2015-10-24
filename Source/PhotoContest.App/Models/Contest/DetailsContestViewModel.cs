using PhotoContest.Models.Enumerations;

namespace PhotoContest.App.Models.Contest
{
    using System;
    using System.Collections.Generic;
    using AutoMapper;
    using Bookmarks.Common.Mappings;
    using PhotoContest.Models;
    using Pictures;

    public class DetailsContestViewModel : IMapFrom<Contest>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Owner { get; set; }

        public bool IsOwner { get; set; }

        public ContestStatus Status { get; set; }

        public VotingType VotingType { get; set; }

        public ParticipationType ParticipationType { get; set; }

        public DeadlineType DeadlineType { get; set; }

        public int PicturesCount { get; set; }

        public int ParticipantsCount { get; set; }

        public int VotesCount { get; set; }

        public IEnumerable<SummaryPictureViewModel> Pictures { get; set; }

        public void CreateMappings(IConfiguration configuration)
        {
        }
    }
}