namespace SyncSharpServer.Models.RequestModels
{
    public class AddDeleteWorkMemberRequestModel
    {
        public Guid WorkID { get; set; }
        public Guid UserID { get; set; }
    }
}
