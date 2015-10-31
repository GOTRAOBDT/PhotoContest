namespace PhotoContest.App.Models.Contest
{
    using Account;

    using PagedList;

    public class CandidatesViewModel
    {
        public IPagedList<BasicUserInfoViewModel> Candidates { get; set; }

        public bool IsContestOwner { get; set; }

        public int? ContestId { get; set; }
    }
}