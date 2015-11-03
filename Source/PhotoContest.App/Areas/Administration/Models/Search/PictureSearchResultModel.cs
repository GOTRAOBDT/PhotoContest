namespace PhotoContest.App.Areas.Administration.Models.Search
{
    public class PictureSearchResultModel : App.Models.Search.PictureSearchResultModel
    {
        public override string ResultUrl()
        {
            return string.Format("/Administration/Pictures/{0}/index", this.Id);
        }
    }
}