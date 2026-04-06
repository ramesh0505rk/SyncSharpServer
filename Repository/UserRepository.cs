using SyncSharpServer.Interfaces;
using SyncSharpServer.Models.ResponseModels;
using SyncSharpServer.Models.RequestModels;

namespace SyncSharpServer.Repository
{
	public class UserRepository : IUserRepository
	{
		public async Task<SignInResponseModel> SignInAsync(SignInRequestModel request)
		{
			return await new Task<SignInResponseModel>(() =>
			{
				return new SignInResponseModel();
			});
		}
	}
}
