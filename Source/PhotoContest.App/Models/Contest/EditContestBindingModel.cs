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

        public int Id { get; set; }

        [Required]
        public string OwnerId { get; set; }

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
        public ContestStatus Status { get; set; }

        [Required]
        [Display(Name="Voting Type")]
        public VotingType VotingType { get; set; }

        [Required]
        [Display(Name = "Participation Type")]
        public ParticipationType ParticipationType { get; set; }

        [Required]
        [Display(Name = "Deadline Type")]
        public DeadlineType DeadlineType { get; set; }

        [Range(1, 1000000)]
        [Display(Name ="Participation Limit")]
        public int? ParticipationLimit { get; set; }

        [Required]
        public string Thumbnail { get; set; }
        
        public IEnumerable<PrizeViewModel> Prizes { get; set; }

        public void CreateMappings(IConfiguration configuration)
        {
        }
    }
}