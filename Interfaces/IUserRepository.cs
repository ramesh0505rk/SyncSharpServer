using SyncSharpServer.Models.RequestModels;
using SyncSharpServer.Models.ResponseModels;

namespace SyncSharpServer.Interfaces
{
    public interface IUserRepository
    {
        Task<SignInResponseModel> SignInAsync(SignInRequestModel request);
        Task<bool> CheckUserExists(string userEmail, CancellationToken cancellationToken);

    }
}
