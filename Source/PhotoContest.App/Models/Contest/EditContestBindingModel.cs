using PhotoContest.Models.Enumerations;

namespace PhotoContest.App.Models.Contest
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using AutoMapper;
    using Bookmarks.Common.Mappings;
    using PhotoContest.Models;

    public class EditContestBindingModel : IMapFrom<Contest>, IHaveCustomMappings
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
        public string Status { get; set; }

        [Required]
        public string ParticipationType { get; set; }

        [Required]
        public string DeadlineType { get; set; }

        [Required]
        public string Thumbnail { get; set; }
        
        public IEnumerable<Prize> Prizes { get; set; }

        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<Contest, EditContestBindingModel>()
                .ForMember(c => c.VotingType, cnf =>
                                cnf.MapFrom(m => m.VotingType.ToString()))
                .ForMember(c => c.DeadlineType, cnf =>
                                cnf.MapFrom(m => m.DeadlineType.ToString()))
                .ForMember(c => c.ParticipationType, cnf =>
                                cnf.MapFrom(m => m.ParticipationType.ToString()))
                .ForMember(c => c.Status, cnf =>
                                cnf.MapFrom(m => m.Status.ToString()));
        }
    }
}