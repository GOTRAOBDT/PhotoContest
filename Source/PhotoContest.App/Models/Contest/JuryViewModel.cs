namespace PhotoContest.App.Models.Contest
{
    using System.Collections.Generic;

    using Account;

    public class JuryViewModel
    {
        public IEnumerable<BasicUserInfoViewModel> Members { get; set; }

        public int? ContestId { get; set; }
    }
}