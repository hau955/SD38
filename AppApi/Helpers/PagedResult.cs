namespace AppApi.Helpers
{
    public class PagedResult<T>
    {
        public List<T>? Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public PagedResult() { }
        public PagedResult(List<T>? items, int totalCount, int pageSize, int currentPage, int totalPage)
        {
            Items = items;
            TotalCount = totalCount;
            PageSize = pageSize;
            CurrentPage = currentPage;
            TotalPages = totalPage;
        }
    }
}
