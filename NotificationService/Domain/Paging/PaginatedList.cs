namespace NotificationService.Domain.Paging
{
    public class PaginatedList<T>
    {
        public List<T> Items { get; }
        public int Page { get; }
        public int TotalPages { get; }
        public int TotalCount { get; }
        public bool HasPreviousPage => Page > 1;
        public bool HasNextPage => Page < TotalPages;

        public PaginatedList(List<T> items, int count, int page, int pageSize)
        {
            Page = page;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalCount = count;
            Items = items;
        }
    }
}