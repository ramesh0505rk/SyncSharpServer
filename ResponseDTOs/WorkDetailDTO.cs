namespace SyncSharpServer.ResponseDTOs
{
    public class WorkDetailDTO
    {
        public Guid WorkID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public Guid CreatedBy { get; set; }
        public string CreatorUserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
        public List<ActiveUserDTO>? ActiveUsers { get; set; }
    }
}
