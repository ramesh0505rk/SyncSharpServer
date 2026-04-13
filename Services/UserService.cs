using SyncSharpServer.Interfaces;
using SyncSharpServer.Models.RequestModels;
using SyncSharpServer.Models.ResponseModels;

namespace SyncSharpServer.Services
{
	public class UserService : IUserService
	{
		private readonly IUserRepository _userRepository;
		private readonly ILogger<UserService> _logger;
		private readonly IConfiguration _configuration;
		public UserService(IUserRepository userRepository, ILogger<UserService> logger, IConfiguration configuration)
		{
			_userRepository = userRepository;
			_logger = logger;
			_configuration = configuration;
		}

		public async Task<SignInResponseModel> SignIn(SignInRequestModel request)
		{
			return await new Task<SignInResponseModel>(() =>
			{
				return new SignInResponseModel();
			});
		}

		public async Task<SignUpResponseModel> SignUp(SignUpRequestModel request)
		{
			return await new Task<SignUpResponseModel>(() =>
			{
				return new SignUpResponseModel();
			});
		}
	}
}
