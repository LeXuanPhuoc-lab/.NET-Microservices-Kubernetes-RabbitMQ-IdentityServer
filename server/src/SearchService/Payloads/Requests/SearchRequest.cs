namespace SearchService.Payloads.Requests
{
    public class SearchRequest
    {
        public int PageIndex { get; set; } = 1;
        public int? PageSize { get; set; }
        public string? SearchValue { get; set; }
        public string? Winner { get; set; } 
        public string? Seller { get; set; } 
        public string? OrderBy { get; set; }
        public string? FilterBy { get; set; }
    }
}