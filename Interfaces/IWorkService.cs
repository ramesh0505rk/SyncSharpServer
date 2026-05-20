using SyncSharpServer.Entities;
using SyncSharpServer.Models;
using SyncSharpServer.Models.RequestModels;
using SyncSharpServer.ResponseDTOs;

namespace SyncSharpServer.Interfaces
{
    public interface IWorkService
    {
        Task<GeneralResponse<WorkDTO>> GetWorkByID(Guid WorkID, CancellationToken cancellationToken);
        Task<GeneralResponse<List<WorkDTO>>> GetUserWorks(Guid UserID, CancellationToken cancellationToken);
        Task<GeneralResponse<Guid>> CreateWork(CreateWorkRequestModel request, CancellationToken cancellationToken);
        Task<GeneralResponse<bool>> UpdateWork(UpdateWorkRequestModel request, CancellationToken cancellationToken);
        Task<GeneralResponse<bool>> DeleteWork(Guid WorkID, CancellationToken cancellationToken);
        Task<GeneralResponse<bool>> AddWorkMember(AddDeleteWorkMemberRequestModel request, CancellationToken cancellationToken);
        Task<GeneralResponse<bool>> DeleteWorkMember(AddDeleteWorkMemberRequestModel request, CancellationToken cancellationToken);
        Task<bool> HasAccess(Guid WorkID, Guid UserID, CancellationToken cancellationToken);
		Task<GeneralResponse<List<User>>> GetMembers(Guid WorkID, CancellationToken cancellationToken);

	}
}
