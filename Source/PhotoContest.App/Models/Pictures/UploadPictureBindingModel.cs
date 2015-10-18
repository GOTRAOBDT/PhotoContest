namespace PhotoContest.App.Models.Pictures
{
    using System.ComponentModel.DataAnnotations;

    public class UploadPictureBindingModel
    {
        [Required]
        public string PictureData { get; set; }
    }
}