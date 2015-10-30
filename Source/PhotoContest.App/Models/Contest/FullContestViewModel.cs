namespace PhotoContest.App.Models.Contest
{
    using System.Collections.Generic;

    public class FullContestViewModel
    {
        public DetailsContestViewModel ContestSummary { get; set; }

        public ICollection<ContestWinnerViewModel> Winners { get; set; }
    }
}