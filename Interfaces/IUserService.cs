using SyncSharpServer.Models.RequestModels;
using SyncSharpServer.Models.ResponseModels;

namespace SyncSharpServer.Interfaces
{
	public interface IUserService
	{
		Task<SignInResponseModel> SignIn(SignInRequestModel request, CancellationToken cancellationToken);
		Task<SignUpResponseModel> SignUp(SignUpRequestModel request, CancellationToken cancellationToken);
	}
}
