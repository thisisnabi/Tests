namespace Tests.Src.UnitTests.Example4;
 
public class PagedList<T>
{
    private PagedList(
        int totalItems,
        IReadOnlyCollection<T> items,
        int pageNumber,
        int pageSize)
    {
        TotalItems = totalItems;
        PageNumber = pageNumber;
        PageSize = pageSize;
        Items = items;
    }

    public static async Task<PagedList<T>> Paginate(
        IQueryable<T> source,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken =default)
    {
        var totalItems = source.Count();
        var items = source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedList<T>(totalItems, items, pageNumber, pageSize);
    }

    public int TotalItems { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public IReadOnlyCollection<T> Items { get; }
    public int TotalPages =>
          (int)Math.Ceiling(TotalItems / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
    public int NextPageNumber =>
           HasNextPage ? PageNumber + 1 : TotalPages;
    public int PreviousPageNumber =>
           HasPreviousPage ? PageNumber - 1 : 1;
}
 