namespace PhotoContest.App.Models.Contest
{
    using System;
    using System.Collections.Generic;

    using Pictures;

    public class DetailsContestViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Owner { get; set; }

        public bool IsOwner { get; set; }

        public string Status { get; set; }

        public string VotingType { get; set; }

        public string ParticipationType { get; set; }

        public string DeadlineType { get; set; }

        public int PicturesCount { get; set; }

        public int ParticipantsCount { get; set; }

        public int VotesCount { get; set; }

        public IEnumerable<SummaryPictureViewModel> Pictures { get; set; }
    }
}