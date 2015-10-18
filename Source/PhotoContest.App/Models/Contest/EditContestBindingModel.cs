namespace PhotoContest.App.Models.Contest
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using PhotoContest.Models;

    public class EditContestBindingModel
    {
        [Required]
        [MinLength(3)]
        public string Title { get; set; }

        [Required]
        [MinLength(3)]
        public string Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public virtual User Owner { get; set; }

        [Required]
        public string VotingType { get; set; }

        [Required]
        public string ParticipationType { get; set; }

        [Required]
        public string DeadlineType { get; set; }

        [Required]
        public string Thumbnail { get; set; }
        
        public IEnumerable<Prize> Prizes { get; set; }
    }
}