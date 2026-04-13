using SyncSharpServer.Models.RequestModels;
using SyncSharpServer.Models.ResponseModels;

namespace SyncSharpServer.Interfaces
{
	public interface IUserService
	{
		Task<SignInResponseModel> SignIn(SignInRequestModel request);
		Task<SignUpResponseModel> SignUp(SignUpRequestModel request);
	}
}
