using SyncSharpServer.Entities;
using SyncSharpServer.Models.RequestModels;
using SyncSharpServer.Models.ResponseModels;

namespace SyncSharpServer.Interfaces
{
    public interface IUserRepository
    {
        Task<SignInResponseModel> SignInAsync(SignInRequestModel request);
        Task<bool> CheckUserExists(string email, CancellationToken cancellationToken);
        Task<User> CreateUser(SignUpRequestModel request, CancellationToken cancellationToken);
        Task<Guid?> ValidateUser(string email, string password, CancellationToken cancellationToken);
        Task<User> GetUserDetailsByUserId(Guid? userId, CancellationToken cancellationToken);

    }
}
