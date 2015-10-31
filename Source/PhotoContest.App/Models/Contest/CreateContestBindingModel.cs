using AutoMapper;
using PhotoContest.Models.Enumerations;

namespace PhotoContest.App.Models.Contest
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Bookmarks.Common.Mappings;

    using PhotoContest.Models;

    public class CreateContestBindingModel : IMapTo<Contest>, IHaveCustomMappings
    {
        [Required]
        [MinLength(3)]
        public string Title { get; set; }

        [Required]
        [MinLength(3)]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]

        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        [Display(Name = "Voting Type")]
        public VotingType VotingType { get; set; }

        [Required]
        [Display(Name = "Participation Type")]
        public ParticipationType ParticipationType { get; set; }

        [Required]
        [Display(Name = "Deadline Type")]
        public DeadlineType DeadlineType { get; set; }

        [Range(1, 1000000)]
        [Display(Name = "Participation Limit")]
        public int? ParticipationLimit { get; set; }

        [Required]
        [Display(Name = "Contest Picture")]
        [StringLength(1398102, ErrorMessage = "The picture exceeds the allowed limit of 1mb.", MinimumLength = 3)]

        public string Thumbnail { get; set; }

        public IEnumerable<Prize> Prizes { get; set; }

        public void CreateMappings(IConfiguration configuration)
        {
        }
    }
}