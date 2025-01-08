namespace UserService.Domain.Paging
{
    public static class PaginationExtensions
    {
        public static PaginatedList<T> ToPaginatedList<T>(this IEnumerable<T> items, long totalItems, int page, int pageSize) where T : notnull
        {
            return new() { Items = items, TotalItems = totalItems, Page = page, PageSize = pageSize };
        }
    }
}
