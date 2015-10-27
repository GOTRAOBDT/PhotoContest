namespace PhotoContest.App.Models.Pictures
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class UploadPictureBindingModel
    {
        [Required]
        [DisplayName("Selected Picture")]
        [StringLength(1398102, ErrorMessage = "The picture exceeds the allowed limit of 1mb.", MinimumLength = 3)]
        public string PictureData { get; set; }

        public string Title { get; set; }
    }
}