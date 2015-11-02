namespace PhotoContest.Models
{
    using System.Collections.Generic;

    using Contracts;

    public class SearchResultsRepository : ISearchResultsRepository
    {
        private List<ISearchResult> results;

        public SearchResultsRepository()
        {
            this.results = new List<ISearchResult>();
        }

        public List<ISearchResult> Results
        {
            get
            {
                return this.results;
            }

            set
            {
                this.results = value;
            }
        }
    }
}
