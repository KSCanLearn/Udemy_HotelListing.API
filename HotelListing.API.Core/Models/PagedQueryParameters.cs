namespace HotelListing.API.Core.Models
{
    public class PagedQueryParameters
    {

        public int StartIndex { get; set; }
        public int PageNumber { get; set; }

        private int _pageSize = 15;

        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = value;
            }
        }

    }

}
