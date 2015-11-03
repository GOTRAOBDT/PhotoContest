namespace PhotoContest.App.Areas.Administration.Models.Search
{
    public class UserSearchResultModel : App.Models.Search.UserSearchResultModel
    {
        public override string ResultUrl()
        {
            return string.Format("/Administration/Users/GetUserByUsername?username={0}", this.UserName);
        }
    }
}