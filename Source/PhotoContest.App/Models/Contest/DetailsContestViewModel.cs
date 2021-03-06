﻿namespace PhotoContest.App.Models.Contest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper;
    using AutoMapper.QueryableExtensions;

    using Bookmarks.Common.Mappings;
    using PhotoContest.Common;
    using PhotoContest.Models;
    using PhotoContest.Models.Enumerations;
    using Pictures;

    public class DetailsContestViewModel : IMapFrom<Contest>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Owner { get; set; }

        public string OwnerUsername { get; set; }

        public bool IsOwner { get; set; }

        public bool CanParticipate { get; set; }

        public bool CanApply { get; set; }

        public ContestStatus Status { get; set; }

        public VotingType VotingType { get; set; }

        public ParticipationType ParticipationType { get; set; }

        public DeadlineType DeadlineType { get; set; }

        public int? ParticipationLimit { get; set; }

        public string Thumbnail { get; set; }

        public IEnumerable<PrizeViewModel> Prizes { get; set; }

        public int PicturesCount { get; set; }

        public int ParticipantsCount { get; set; }

        public int VotesCount { get; set; }

        public IEnumerable<SummaryPictureViewModel> Pictures { get; set; }

        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<Contest, DetailsContestViewModel>()
                .ForMember(c => c.Owner, cfg => cfg.MapFrom(c => c.Owner.Name ?? c.Owner.UserName))
                .ForMember(c => c.OwnerUsername, cfg => cfg.MapFrom(c => c.Owner.UserName))
                .ForMember(c => c.Thumbnail, cnf => cnf.MapFrom(m => m.Picture ?? (m.Thumbnail ?? GlobalConstants.DefaultContestThumbnail)))
                .ForMember(c => c.PicturesCount, cfg => cfg.MapFrom(c => c.Pictures.Count))
                .ForMember(c => c.VotesCount, cfg => cfg.MapFrom(c => c.Votes.Count))
                .ForMember(c => c.Prizes, cfg => cfg.MapFrom(c => c.Prizes.AsQueryable().ProjectTo<PrizeViewModel>()))
                .ForMember(c => c.ParticipantsCount, cfg => cfg.MapFrom(c => c.Participants.Count));
        }
    }
}