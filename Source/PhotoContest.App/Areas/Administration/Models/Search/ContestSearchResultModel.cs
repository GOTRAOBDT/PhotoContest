namespace PhotoContest.App.Areas.Administration.Models.Search
{
    public class ContestSearchResultModel : App.Models.Search.ContestSearchResultModel
    {
        public override string ResultUrl()
        {
            return string.Format("/Administration/contests/{0}/GetContestById", this.Id);
        }
    }
}