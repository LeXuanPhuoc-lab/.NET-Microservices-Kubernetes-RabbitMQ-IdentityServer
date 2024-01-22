namespace SearchService.Payloads.Responses
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; } 
        public string Message { get; set; } = string.Empty;
        public IDictionary<string, string[]> Errors { get; set; } = null!;
    }
}