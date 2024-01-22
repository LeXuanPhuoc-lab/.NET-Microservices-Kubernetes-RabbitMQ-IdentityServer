using System.Collections;

namespace SearchService.Utils
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex {get;set;}
        public int TotalPage {get;set;}

        public PaginatedList(List<T> items, int pageIndex, int totalPage)
        {
            PageIndex = pageIndex;
            TotalPage = totalPage;
            AddRange(items);
        }

        public static PaginatedList<T> Paging(IEnumerable<T> source, int pageIndex, int pageSize)
        {
            // Get total page 
            var totalPage = (int)Math.Ceiling(source.Count() / (double)pageSize);

            // Invalid pageIndex
            if(pageIndex > totalPage || pageIndex < 0) pageIndex = 1; // set to default

            // Get items by pageIndex, pageSize
            var items = source.Skip((pageIndex - 1) * pageSize) // offset
                              .Take(pageSize)
                              .ToList();

            // Paging
            return new PaginatedList<T>(items, pageIndex, totalPage);
        }
    }
}