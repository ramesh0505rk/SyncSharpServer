namespace SyncSharpServer.Entities
{
	public class ActiveSession
	{
		public Guid WorkID { get; set; }
		public Guid UserID { get; set; }
		public string ConnectionID { get; set; } = string.Empty;
		public string UserName { get; set; } = string.Empty;
		public DateTime JoinedAt { get; set; }
		public DateTime LastActivity { get; set; }
	}
}
