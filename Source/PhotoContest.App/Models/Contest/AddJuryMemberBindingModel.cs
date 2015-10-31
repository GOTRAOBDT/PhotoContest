namespace PhotoContest.App.Models.Contest
{
    using System.ComponentModel.DataAnnotations;

    public class AddJuryMemberBindingModel
    {
        [Required]
        public int ContestId { get; set; }

        [Required]
        public string Username { get; set; }
    }
}