namespace PhotoContest.App.Models.Account
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Web;

    using PhotoContest.Models;
    using PhotoContest.Models.Enumerations;
    using Bookmarks.Common.Mappings;
    

    public class EditProfileBindingModel : IMapFrom<User>, IHaveCustomMappings
    {
        [Required]
        [StringLength(255, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }


        [EnumDataType(typeof(UserGender), ErrorMessage = "Invalid Gender value.")]
        public UserGender Gender { get; set; }

        [Display(Name ="Picture")]
        [StringLength(98304, ErrorMessage = "The picture exceeds the allowed limit of 128kb.", MinimumLength = 3)]
        public string ProfilePicture { get; set; }

        public void CreateMappings(AutoMapper.IConfiguration configuration)
        {
            configuration.CreateMap<User, EditProfileBindingModel>()
                .ForMember(u => u.Name, cfg => cfg.MapFrom(e => e.Name))
                .ForMember(u => u.Email, cfg => cfg.MapFrom(e => e.Email))
                .ForMember(u => u.BirthDate, cfg => cfg.MapFrom(e => e.BirthDate))
                .ForMember(u => u.ProfilePicture, cfg => cfg.MapFrom(e => e.ProfilePicture))
                .ForMember(u => u.Gender, cfg => cfg.MapFrom(e => e.Gender));
        }
    }
}