using Dapper;
using Newtonsoft.Json;
using SyncSharpServer.Entities;
using SyncSharpServer.Interfaces;
using SyncSharpServer.Models.RequestModels;
using SyncSharpServer.Presistence;
using SyncSharpServer.ResponseDTOs;

namespace SyncSharpServer.Repository
{
	public class WorkRepository : IWorkRepository
	{
		private readonly IDbConnectionFactory _dbConnectionFactory;
		private readonly ILogger<WorkRepository> _logger;

		public WorkRepository(IDbConnectionFactory dbConnectionFactory, ILogger<WorkRepository> logger)
		{
			_dbConnectionFactory = dbConnectionFactory;
			_logger = logger;
		}

		public async Task<WorkDTO?> GetWorkByID(Guid WorkID, CancellationToken cancellationToken)
		{
			try
			{
				using var connection = await _dbConnectionFactory.GetOpenConnection(cancellationToken);
				var query = "SELECT * FROM [Work] WHERE WorkID = @WorkID";

				var parameters = new DynamicParameters();
				parameters.Add("@WorkID", WorkID);

				var result = await connection.QueryFirstOrDefaultAsync<WorkDTO>(query, parameters, commandType: System.Data.CommandType.Text);
				return result;
			}
			catch (Exception ex)
			{
				var inputParams = new { WorkID };
				_logger.LogError(ex, "Error thrown in WorkRepository.GetWorkByID. Input parameters: {InputParams}", JsonConvert.SerializeObject(inputParams));
				throw;
			}
		}

		public async Task<List<WorkDTO>> GetUserWorks(Guid UserID, CancellationToken cancellationToken)
		{
			try
			{
				using var connection = await _dbConnectionFactory.GetOpenConnection(cancellationToken);
				var query = @"
                            SELECT DISTINCT w.* 
                            FROM Work w
                            LEFT JOIN WorkMembers wm ON w.WorkID = wm.WorkID
                            WHERE w.CreatedBy = @UserID or wm.UserID = @UserID
                            ORDER BY w.LastModified DESC
                            ";
				var parameters = new DynamicParameters();
				parameters.Add("@UserID", UserID);

				var result = await connection.QueryAsync<WorkDTO>(query, parameters, commandType: System.Data.CommandType.Text);
				return result.ToList();

			}
			catch (Exception ex)
			{
				var inputParams = new { UserID };
				_logger.LogError(ex, "Error thrown in WorkRepository.GetUserWorks. Input parameters: {InputParams}", JsonConvert.SerializeObject(inputParams));
				throw;
			}
		}

		public async Task<Guid> CreateWork(CreateWorkRequestModel request, CancellationToken cancellationToken)
		{
			try
			{
				using var connection = await _dbConnectionFactory.GetOpenConnection(cancellationToken);
				var query = @"
                            INSERT INTO [Work] (WorkID, Title, Description, Code, Language, CreatedBy)
                            OUTPUT INSERTED.WorkID
                            VALUES (@WorkID, @Title, @Description, @Code, @Language, @CreatedBy);
                            ";

				var parameters = new DynamicParameters();
				parameters.Add("@WorkID", Guid.NewGuid());
				parameters.Add("@Title", request.Title);
				parameters.Add("@Description", request.Description);
				parameters.Add("@Code", request.Code);
				parameters.Add("@Language", request.Language);
				parameters.Add("@CreatedBy", request.CreatedBy);

				var result = await connection.ExecuteScalarAsync<Guid>(query, parameters, commandType: System.Data.CommandType.Text);
				return result;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error thrown in WorkRepository.CreateWork. Input parameters: {InputParams}", JsonConvert.SerializeObject(request));
				throw;
			}
		}

		public async Task<bool> UpdateWork(UpdateWorkRequestModel request, CancellationToken cancellationToken)
		{
			try
			{
				using var connection = await _dbConnectionFactory.GetOpenConnection(cancellationToken);
				var query = @"
                            UPDATE Work 
                            SET Code = @Code, [Language] = @Language, LastModified = GETDATE(), ModifiedBy = @ModifiedBy 
                            WHERE WorkID = @WorkID
                            ";

				var parameters = new DynamicParameters();
				parameters.Add("@Code", request.Code);
				parameters.Add("@Language", request.Language);
				parameters.Add("@ModifiedBy", request.ModifiedBy);
				parameters.Add("@WorkID", request.WorkID);

				var result = await connection.ExecuteAsync(query, parameters, commandType: System.Data.CommandType.Text);
				return result > 0;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error thrown in WorkRepository.UpdateWork. Input parameters: {InputParams}", JsonConvert.SerializeObject(request));
				throw;
			}
		}

		public async Task<bool> DeleteWork(Guid WorkID, CancellationToken cancellationToken)
		{
			try
			{
				using var connection = await _dbConnectionFactory.GetOpenConnection(cancellationToken);
				var query = "DELETE FROM Work WHERE WorkID = @WorkID";

				var parameters = new DynamicParameters();
				parameters.Add("@WorkID", WorkID);

				var result = await connection.ExecuteAsync(query, parameters, commandType: System.Data.CommandType.Text);
				return result > 0;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error thrown in WorkRepository.DeleteWork. Input parameters: {InputParams}", JsonConvert.SerializeObject(new { WorkID }));
				throw;
			}
		}

		public async Task<bool> AddWorkMember(AddDeleteWorkMemberRequestModel request, CancellationToken cancellationToken)
		{
			try
			{
				using var connection = await _dbConnectionFactory.GetOpenConnection(cancellationToken);
				var query = @"
                            IF NOT EXISTS (SELECT 1 FROM WorkMembers WHERE WorkID = @WorkID AND UserID = @UserID)
                            BEGIN
                                INSERT INTO WorkMembers (WorkID, UserID) VALUES (@WorkID, @UserID)
                            END
                            ";
				var parameters = new DynamicParameters();
				parameters.Add("@WorkID", request.WorkID);
				parameters.Add("@UserID", request.UserID);

				var result = await connection.ExecuteAsync(query, parameters, commandType: System.Data.CommandType.Text);
				return result > 0;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error thrown in WorkRepository.AddWorkMember. Input parameters: {InputParams}", JsonConvert.SerializeObject(request));
				throw;
			}
		}

		public async Task<bool> DeleteWorkMember(AddDeleteWorkMemberRequestModel request, CancellationToken cancellationToken)
		{
			try
			{
				using var connection = await _dbConnectionFactory.GetOpenConnection(cancellationToken);
				var query = "DELETE FROM WorkMembers WHERE WorkID = @WorkID AND UserID = @UserID";

				var parameters = new DynamicParameters();
				parameters.Add("@WorkID", request.WorkID);
				parameters.Add("@UserID", request.UserID);

				var result = await connection.ExecuteAsync(query, parameters, commandType: System.Data.CommandType.Text);
				return result > 0;

			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error thrown in WorkRepository.DeleteWorkMember. Input parameters: {InputParams}", JsonConvert.SerializeObject(request));
				throw;
			}
		}

		public async Task<bool> HasAccess(Guid WorkID, Guid UserID, CancellationToken cancellationToken)
		{
			try
			{
				using var connection = await _dbConnectionFactory.GetOpenConnection(cancellationToken);
				var query = @"
							SELECT CASE 
								WHEN EXISTS (
									SELECT 1 FROM Work WHERE WorkID = @WorkId AND CreatedBy = @UserID
								) THEN 1
								WHEN EXISTS (
									SELECT 1 FROM WorkMembers WHERE WorkId = @WorkId AND UserId = @UserID
								) THEN 1
								ELSE 0 
							END AS HasAccess
							";

				var parameters = new DynamicParameters();
				parameters.Add("@WorkId", WorkID);
				parameters.Add("@UserID", UserID);

				return await connection.ExecuteScalarAsync<bool>(query, parameters, commandType: System.Data.CommandType.Text);
			}
			catch (Exception ex)
			{
				var inputParams = new { WorkID, UserID };
				_logger.LogError(ex, "Error thrown in WorkRepository.HasAccess. Input parameters: {InputParams}", JsonConvert.SerializeObject(inputParams));
				throw;
			}
		}

		public async Task<List<User>> GetMembers(Guid WorkID, CancellationToken cancellationToken)
		{
			try
			{
				using var connection = await _dbConnectionFactory.GetOpenConnection(cancellationToken);
				var query = @"
							SELECT u.* 
								FROM [User] u
								INNER JOIN WorkMembers wc ON u.UserID = wc.UserID
								WHERE wc.WorkID = @WorkID
							";

				var parameters = new DynamicParameters();
				parameters.Add("@WorkID", WorkID);

				var result = await connection.QueryAsync<User>(query, parameters, commandType: System.Data.CommandType.Text);
				return result.ToList();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error thrown in WorkRepository.GetMembers. Input parameters: {InputParams}", JsonConvert.SerializeObject(new { WorkID }));
				throw;
			}
		}
	}
}
