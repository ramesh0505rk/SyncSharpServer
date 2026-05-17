namespace SyncSharpServer.Models
{
    public class GeneralResponse<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; }
        public string RequestId { get; set; } = string.Empty;
        public string ResponseMessage { get; set; } = string.Empty;
    }
}
