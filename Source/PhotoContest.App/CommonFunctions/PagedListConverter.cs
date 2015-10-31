namespace PhotoContest.App.CommonFunctions
{
    using System.Linq;

    using AutoMapper;

    using Models.Account;
    using PagedList;
    using PhotoContest.Models;

    public class PagedListConverter : ITypeConverter<IPagedList<User>, IPagedList<BasicUserInfoViewModel>>
    {
        public IPagedList<BasicUserInfoViewModel> Convert(ResolutionContext context)
        {
            var models = (IPagedList<User>)context.SourceValue;
            var viewModels = models.Select(p => Mapper.Map<User, BasicUserInfoViewModel>(p)).ToList();
            return new StaticPagedList<BasicUserInfoViewModel>(viewModels, models.PageNumber, models.PageSize, models.TotalItemCount);
        }
    }
}