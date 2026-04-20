using System.Data;

namespace SyncSharpServer.Presistence
{
    public interface IDbConnectionFactory
    {
        Task<IDbConnection> GetOpenConnection(CancellationToken cancellationToken);
    }
}
