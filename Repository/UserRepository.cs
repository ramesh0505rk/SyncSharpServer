using SyncSharpServer.Interfaces;
using SyncSharpServer.Models.ResponseModels;
using SyncSharpServer.Models.RequestModels;
using SyncSharpServer.Presistence;
using Newtonsoft.Json;
using Dapper;

namespace SyncSharpServer.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(IDbConnectionFactory dbConnectionFactory, ILogger<UserRepository> logger)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _logger = logger;
        }
        public async Task<SignInResponseModel> SignInAsync(SignInRequestModel request)
        {
            return await new Task<SignInResponseModel>(() =>
            {
                return new SignInResponseModel();
            });
        }

        public async Task<SignUpResponseModel> SignUpAsync(SignUpRequestModel request)
        {
            return await new Task<SignUpResponseModel>(() =>
            {
                return new SignUpResponseModel();
            });
        }

        public async Task<bool> CheckUserExists(string userEmail, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Information in UserRepository.CheckUserExists. Input parameters: {userEmail}", userEmail);
                using var connection = await _dbConnectionFactory.GetOpenConnection(cancellationToken);

                var query = "SELECT COUNT(1) FROM Users WHERE Email = @UserEmail";

                var parameters = new DynamicParameters();
                parameters.Add("@UserEmail", userEmail);

                var result = await connection.QueryFirstOrDefaultAsync<int>(query, parameters, commandType: System.Data.CommandType.StoredProcedure);

                return result > 0;
            }
            catch (Exception ex)
            {
                var inputParams = new { userEmail };
                _logger.LogError(ex, "Error thrown in UserRepository.CheckUserExists. Input parameters: {InputParams}", JsonConvert.SerializeObject(inputParams));
                throw;
            }
        }
    }
}
