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
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public string OwnerId { get; set; }

        [Required]
        public string VotingType { get; set; }

        [Required]
        public string ParticipationType { get; set; }

        [Required]
        public string DeadlineType { get; set; }

        [Required]
        public string Thumbnail { get; set; }

        public IEnumerable<Prize> Prizes { get; set; }

        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<CreateContestBindingModel, Contest>()
                .ForMember(c => c.VotingType, cnf => 
                                cnf.MapFrom(m => (VotingType)Enum.Parse(typeof(VotingType), m.VotingType)))
                .ForMember(c => c.DeadlineType, cnf => 
                                cnf.MapFrom(m => (DeadlineType)Enum.Parse(typeof(DeadlineType), m.DeadlineType)))
                .ForMember(c => c.Status, cnf =>
                                cnf.MapFrom(m => (ParticipationType)Enum.Parse(typeof(ParticipationType), m.ParticipationType)));
        }
    }
}