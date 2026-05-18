using SyncSharpServer.Models;
using SyncSharpServer.Models.RequestModels;
using SyncSharpServer.ResponseDTOs;

namespace SyncSharpServer.Interfaces
{
    public interface IWorkRepository
    {
        Task<WorkDTO?> GetWorkByID(Guid WorkID, CancellationToken cancellationToken);
        Task<List<WorkDTO>> GetUserWorks(Guid UserID, CancellationToken cancellationToken);
        Task<Guid> CreateWork(CreateWorkRequestModel request, CancellationToken cancellationToken);
        Task<bool> UpdateWork(UpdateWorkRequestModel request, CancellationToken cancellationToken);
        Task<bool> DeleteWork(Guid WorkID, CancellationToken cancellationToken);
        Task<bool> AddWorkMember(AddDeleteWorkMemberRequestModel request, CancellationToken cancellationToken);
        Task<bool> DeleteWorkMember(AddDeleteWorkMemberRequestModel request, CancellationToken cancellationToken);
    }
}
