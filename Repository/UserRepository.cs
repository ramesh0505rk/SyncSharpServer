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

		public async Task<User> CreateUser(SignUpRequestModel request, CancellationToken cancellationToken)
		{
			try
			{
				_logger.LogInformation("Information in UserRepository.CreateUser. Input parameters: {InputParams}", JsonConvert.SerializeObject(request));
				using var connection = await _dbConnectionFactory.GetOpenConnection(cancellationToken);

				var query = "INSERT INTO SyncSharp.dbo.[user] (UserID, FirstName, LastName, Email, Password) OUTPUT INSERTED.* VALUES (@UserID, @FirstName, @LastName, @Email, @Password);";

				var parameters = new DynamicParameters();
				parameters.Add("@UserID", Guid.NewGuid());
				parameters.Add("@FirstName", request.FirstName);
				parameters.Add("@LastName", request.LastName);
				parameters.Add("@Email", request.Email);
				parameters.Add("@Password", request.Password);

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

				var query = @"SELECT UserID FROM SyncSharp.dbo.[user] WHERE 
							Email = @Email AND [Password] = @Password";

				var parameters = new DynamicParameters();
				parameters.Add("@Email", email);
				parameters.Add("@Password", password);

				var result = await connection.QueryFirstOrDefaultAsync<Guid?>(query, parameters);
				return result;
			}
			catch (Exception ex)
			{
				var inputParams = new { email };
				_logger.LogError(ex, "Error thrown in UserRepository.ValidateUser. Input parameters: {InputParams}", JsonConvert.SerializeObject(inputParams));
				throw;
			}
		}
	}
}
