namespace SearchService.Payloads.Responses
{
    public class BaseResponse
    {
        public int StatusCode { get; set; } 
        public string Message { get; set; } = string.Empty;
        public object Data { get; set; } = null!;
    }
}