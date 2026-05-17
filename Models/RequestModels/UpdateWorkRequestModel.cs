namespace SyncSharpServer.Models.RequestModels
{
    public class UpdateWorkRequestModel
    {
        public Guid WorkID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public Guid ModifiedBy { get; set; }
    }
}
