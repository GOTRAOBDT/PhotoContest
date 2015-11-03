namespace PhotoContest.App.Models.Contest
{
    using Bookmarks.Common.Mappings;
    using PhotoContest.App.Models.Pictures;

    public class GalleryViewModel
    {
        public int ContestId { get; set; }

        public int PicturesCount { get; set; }

        public int CurrentPictureIndex { get; set; }

        public int? PreviousPictureId { get; set; }

        public int? NextPictureId { get; set; }

        public DetailsPictureViewModel CurrentPicture { get; set; }
    }
}