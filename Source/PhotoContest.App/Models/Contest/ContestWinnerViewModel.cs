namespace PhotoContest.App.Models.Contest
{
    using PhotoContest.App.Models.Pictures;

    public class ContestWinnerViewModel
    {
        public int PictureId { get; set; }

        public int ContestId { get; set; }

        public string WinnerName { get; set; }

        public string WinnerUsername { get; set; }

        public string PrizeName { get; set; }

        public SummaryPictureViewModel Picture { get; set; }
    }
}