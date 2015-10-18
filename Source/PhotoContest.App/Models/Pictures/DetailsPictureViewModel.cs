namespace PhotoContest.App.Models.Pictures
{
    public class DetailsPictureViewModel
    {
        public int Id { get; set; }

        public string Author { get; set; }

        public bool IsAuthor { get; set; }

        public string PictureData { get; set; }

        public int VotesCount { get; set; }
    }
}