public static class LinqUtil
{
    public static IQueryable<TT> Paginate<TT>(this IQueryable<TT> content, int pageNumber, int pageSize)
    {
        return content.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }

    public static IEnumerable<TT> Paginate<TT>(this IEnumerable<TT> content, int pageNumber, int pageSize)
    {
        return content.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }
}
