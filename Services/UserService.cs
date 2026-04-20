using SyncSharpServer.Interfaces;
using SyncSharpServer.Models.RequestModels;
using SyncSharpServer.Models.ResponseModels;
using SyncSharpServer.Common.ExceptionHandling;

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

        public async Task<SignInResponseModel> SignIn(SignInRequestModel request, CancellationToken cancellationToken)
        {
            return await new Task<SignInResponseModel>(() =>
            {
                return new SignInResponseModel();
            });
        }

        public async Task<SignUpResponseModel> SignUp(SignUpRequestModel request, CancellationToken cancellationToken)
        {
            await CheckUserExists(request, cancellationToken);
        }

        private async Task CheckUserExists(SignUpRequestModel request, CancellationToken cancellationToken)
        {
            try
            {
                var userExists = await _userRepository.CheckUserExists(request.Email, cancellationToken);
                if (userExists)
                {
                    _logger.LogError("User with email {Email} already exists. Input parameters: {InputParams}", request.Email, request);
                    throw new BadRequestException(["User with this email already exists."]);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error thrown in UserService.CheckUserExists. Input parameters: {InputParams}", request);
                throw;
            }
        }
    }
}
