namespace PhotoContest.App.Models.Contest
{
    using Account;

    using PagedList;

    public class ParticipantsViewModel
    {
        public IPagedList<BasicUserInfoViewModel> Participants { get; set; }

        public bool IsContestOwner { get; set; }

        public int? ContestId { get; set; }
    }
}