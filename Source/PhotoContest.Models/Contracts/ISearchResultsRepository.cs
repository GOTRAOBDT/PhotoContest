namespace PhotoContest.Models.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface ISearchResultsRepository
    {
        List<ISearchResult> Results { get; set; }
    }
}
