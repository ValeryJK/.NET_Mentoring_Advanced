namespace Catalog.Domain.Common
{
    public class PagedResponse<T>
    {
        public IEnumerable<T> Data { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)this.TotalCount / this.PageSize);

        public PagedResponse(IEnumerable<T> data, int pageNumber, int pageSize, int totalCount)
        {
            this.Data = data;
            this.PageNumber = pageNumber;
            this.PageSize = pageSize;
            this.TotalCount = totalCount;
        }
    }
}