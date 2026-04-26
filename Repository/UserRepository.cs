using SyncSharpServer.Interfaces;
using SyncSharpServer.Models.ResponseModels;
using SyncSharpServer.Models.RequestModels;
using SyncSharpServer.Presistence;
using Newtonsoft.Json;
using Dapper;
using SyncSharpServer.Entities;

namespace SyncSharpServer.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly ILogger<UserRepository> _logger;
        private readonly IPasswordHasher _passwordHasher;

        public UserRepository(IDbConnectionFactory dbConnectionFactory, ILogger<UserRepository> logger, IPasswordHasher passwordHasher)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _logger = logger;
            _passwordHasher = passwordHasher;
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

        public async Task<bool> CheckUserExists(string email, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Information in UserRepository.CheckUserExists. Input parameters: {userEmail}", email);
                using var connection = await _dbConnectionFactory.GetOpenConnection(cancellationToken);

                var query = "SELECT COUNT(1) FROM [User] WHERE Email = @Email";

                var parameters = new DynamicParameters();
                parameters.Add("@Email", email);

                var result = await connection.QueryFirstOrDefaultAsync<int>(query, parameters, commandType: System.Data.CommandType.Text);

                return result > 0;
            }
            catch (Exception ex)
            {
                var inputParams = new { email };
                _logger.LogError(ex, "Error thrown in UserRepository.CheckUserExists. Input parameters: {InputParams}", JsonConvert.SerializeObject(inputParams));
                throw;
            }
        }

        public async Task<User> CreateUser(SignUpRequestModel request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Information in UserRepository.CreateUser. Input parameters: {InputParams}", JsonConvert.SerializeObject(request));
                using var connection = await _dbConnectionFactory.GetOpenConnection(cancellationToken);

                var hashedPassword = _passwordHasher.HashPassword(request.Password);

                var query = "INSERT INTO SyncSharp.dbo.[user] (UserID, FirstName, LastName, Email, Password) OUTPUT INSERTED.* VALUES (@UserID, @FirstName, @LastName, @Email, @Password);";

                var parameters = new DynamicParameters();
                parameters.Add("@UserID", Guid.NewGuid());
                parameters.Add("@FirstName", request.FirstName);
                parameters.Add("@LastName", request.LastName);
                parameters.Add("@Email", request.Email);
                parameters.Add("@Password", hashedPassword);

                var result = await connection.QueryFirstOrDefaultAsync<User>(query, parameters);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error thrown in UserRepository.CreateUser. Input parameters: {InputParams}", JsonConvert.SerializeObject(request));
                throw;
            }
        }

		public async Task<Guid?> ValidateUser(string email, string password, CancellationToken cancellationToken)
		{
			try
			{
				_logger.LogInformation("Information in UserRepository.ValidateUser. Input parameters: Email = {email}", email);
				using var connection = await _dbConnectionFactory.GetOpenConnection(cancellationToken);

				var query = @"SELECT UserID, [Password] FROM SyncSharp.dbo.[user] WHERE Email = @Email";

				var parameters = new DynamicParameters();
				parameters.Add("@Email", email);

				var user = await connection.QueryFirstOrDefaultAsync<(Guid? UserId, string Password)>(query, parameters);

				if (user.UserId == null || string.IsNullOrEmpty(user.Password))
				{
					return null;
				}

				var isPasswordValid = _passwordHasher.VerifyPassword(password, user.Password);
				return isPasswordValid ? user.UserId : null;
			}
			catch (Exception ex)
			{
				var inputParams = new { email };
				_logger.LogError(ex, "Error thrown in UserRepository.ValidateUser. Input parameters: {InputParams}", JsonConvert.SerializeObject(inputParams));
				throw;
			}
		}

        public async Task<User> GetUserDetailsByUserId(Guid? userId, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Information in UserRepository.GetUserDetailsByUserId. Input params: UserId = {userId}", userId);
                using var connection = await _dbConnectionFactory.GetOpenConnection(cancellationToken);

                var query = @"SELECT UserID, FirstName, LastName, Email 
                            FROM [User] WHERE UserID = @UserID";

                var parameters = new DynamicParameters();
                parameters.Add("@UserID", userId);

                var result = await connection.QueryFirstOrDefaultAsync<User>(query, parameters);

                return result;
            }
            catch (Exception ex)
            {
                var inputParams = new { userId };
                _logger.LogError(ex, "Error thrown in UserRepository.GetUserDetailsByUserId. Input parameters: {InputParams}", JsonConvert.SerializeObject(inputParams));
                throw;
            }
        }
    }
}
