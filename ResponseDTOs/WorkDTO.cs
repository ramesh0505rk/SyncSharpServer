namespace SyncSharpServer.ResponseDTOs
{
    public class WorkDTO
    {
        public Guid WorkID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public string Language { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
        public int? ActiveUsersCount { get; set; } = 0;
    }
}
