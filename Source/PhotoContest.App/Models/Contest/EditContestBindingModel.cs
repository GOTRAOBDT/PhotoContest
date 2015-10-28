namespace PhotoContest.App.Models.Contest
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using AutoMapper;
    using Bookmarks.Common.Mappings;
    using PhotoContest.Models.Enumerations;
    using PhotoContest.Models;

    public class EditContestBindingModel : IMapFrom<Contest>, IHaveCustomMappings
    {
        public EditContestBindingModel()
        {
            this.Prizes = new HashSet<PrizeViewModel>();
        }

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
        public VotingType VotingType { get; set; }

        [Required]
        public ContestStatus Status { get; set; }

        [Required]
        public ParticipationType ParticipationType { get; set; }

        [Required]
        public DeadlineType DeadlineType { get; set; }

        [Required]
        public string Thumbnail { get; set; }
        
        public IEnumerable<PrizeViewModel> Prizes { get; set; }

        public void CreateMappings(IConfiguration configuration)
        {
            
        }
    }
}