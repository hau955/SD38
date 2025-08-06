namespace AppApi.Helpers
{
    public class PagedResult<T>
    {
        public List<T>? Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        // Tiện ích phân trang
        public bool HasNextPage => CurrentPage < TotalPages;
        public bool HasPreviousPage => CurrentPage > 1;

        public PagedResult() { }

        public PagedResult(List<T>? items, int totalCount, int pageSize, int currentPage)
        {
            Items = items;
            TotalCount = totalCount;
            PageSize = pageSize;
            CurrentPage = currentPage;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        }

        public PagedResult(List<T>? items, int totalCount, int pageSize, int currentPage, int totalPages)
        {
            Items = items;
            TotalCount = totalCount;
            PageSize = pageSize;
            CurrentPage = currentPage;
            TotalPages = totalPages;
        }
    }

}
