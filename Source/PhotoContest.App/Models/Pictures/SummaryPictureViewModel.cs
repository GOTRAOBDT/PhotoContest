namespace PhotoContest.App.Models.Pictures
{
    public class SummaryPictureViewModel
    {   
        public int Id { get; set; }
        
        public string Author { get; set; }

        public bool IsAuthor { get; set; }
        
        public string PictureData { get; set; }

        public int ContestsCount { get; set; }

        public int VotesCount { get; set; }
    }
}