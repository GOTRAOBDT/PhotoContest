namespace PhotoContest.App.Models.Account
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class EditProfileBindingModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public DateTime BirthDate { get; set; }

        [Required]
        public string Gender { get; set; }

        public string ProfilePicture { get; set; }
    }
}