namespace PhotoContest.App.Models.Contest
{
    using PhotoContest.App.Models.Pictures;

    public class GalleryViewModel
    {
        public int? PreviousPictureId { get; set; }

        public int? NextPictureId { get; set; }

        public DetailsPictureViewModel CurrentPicture { get; set; }
    }
}