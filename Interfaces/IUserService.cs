using SyncSharpServer.Models.RequestModels;
using SyncSharpServer.Models.ResponseModels;
using SyncSharpServer.ResponseDTOs;

namespace SyncSharpServer.Interfaces
{
	public interface IUserService
	{
		Task<SignInResponseModel> SignIn(SignInRequestModel request, CancellationToken cancellationToken);
		Task<SignUpDTO> SignUp(SignUpRequestModel request, CancellationToken cancellationToken);
	}
}
