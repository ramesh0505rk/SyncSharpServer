namespace SyncSharpServer.Entities
{
    public class WorkVersion
    {
        public int VersionID { get; set; }
        public Guid WorkID { get; set; }
        public string Code { get; set; } = string.Empty;
        public Guid ModifiedBy { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string ChangeDescription { get; set; } = string.Empty;
    }
}
