namespace SyncSharpServer.ResponseDTOs
{
    public class ActiveUserDTO
    {
        public Guid UserID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string ConnectionID { get; set; } = string.Empty;
    }
}
