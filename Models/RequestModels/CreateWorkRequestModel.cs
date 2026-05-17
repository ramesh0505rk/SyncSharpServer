namespace SyncSharpServer.Models.RequestModels
{
    public class CreateWorkRequestModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public Guid CreatedBy { get; set; }
    }
}
