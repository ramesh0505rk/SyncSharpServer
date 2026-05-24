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
                            SELECT DISTINCT w.*,
                                (
                                    SELECT COUNT(as1.UserID) 
                                    FROM ActiveSessions as1 WHERE WorkID = w.WorkID
                                )
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

        public async Task<bool> SaveActiveSession(Guid WorkID, Guid UserID, string ConnectionID, string UserName, CancellationToken cancellationToken)
        {
            try
            {
                using var connection = await _dbConnectionFactory.GetOpenConnection(cancellationToken);
                var query = @"
                            INSERT INTO ActiveSessions (WorkID, UserID, ConnectionID, Username)
                            VALUES (@WorkID, @UserID, @ConnectionID, @Username)
                            ";

                var parameters = new DynamicParameters();
                parameters.Add("@WorkID", WorkID);
                parameters.Add("@UserID", UserID);
                parameters.Add("@ConnectionID", ConnectionID);
                parameters.Add("@Username", UserName);

                var result = await connection.ExecuteAsync(query, parameters, commandType: System.Data.CommandType.Text);
                return result > 0;
            }
            catch (Exception ex)
            {
                var inputParams = new { WorkID, UserID, ConnectionID, UserName };
                _logger.LogError(ex, "Error thrown in WorkRepository.SaveActiveSession. Input parameters: {InputParams}", JsonConvert.SerializeObject(inputParams));
                throw;
            }
        }

        public async Task<bool> RemoveActiveSessionAsync(string connectionID, CancellationToken cancellationToken)
        {
            try
            {
                using var connection = await _dbConnectionFactory.GetOpenConnection(cancellationToken);
                var query = "DELETE FROM ActiveSessions WHERE ConnectionID = @ConnectionID";

                var parameters = new DynamicParameters();
                parameters.Add("@ConnectionID", connectionID);

                var result = await connection.ExecuteAsync(query, parameters, commandType: System.Data.CommandType.Text);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error thrown in WorkRepository.SaveActiveSession. Input parameters: {InputParams}", JsonConvert.SerializeObject(new { connectionID }));
                throw;
            }
        }

        public async Task<ActiveSession?> GetSessionByConnectionID(string connectionID, CancellationToken cancellationToken)
        {
            try
            {
                using var connection = await _dbConnectionFactory.GetOpenConnection(cancellationToken);
                var query = "SELECT * FROM ActiveSessions WHERE ConnectionId = @ConnectionID";

                var parameters = new DynamicParameters();
                parameters.Add("@ConnectionID", connectionID);

                return await connection.QuerySingleOrDefaultAsync<ActiveSession>(query, parameters, commandType: System.Data.CommandType.Text);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error thrown in WorkRepository.GetSessionByConnectionID. Input parameters: {InputParams}", JsonConvert.SerializeObject(new { connectionID }));
                throw;
            }
        }

        public async Task<List<ActiveSession>> GetActiveSessions(Guid WorkID, CancellationToken cancellationToken)
        {
            try
            {
                using var connection = await _dbConnectionFactory.GetOpenConnection(cancellationToken);
                var query = "SELECT * FROM ActiveSessions WHERE WorkID = @WorkID ORDER BY JoinedAt";
                var parameters = new DynamicParameters();
                parameters.Add("@WorkID", WorkID);

                var result = await connection.QueryAsync<ActiveSession>(query, parameters, commandType: System.Data.CommandType.Text);
                return result.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error thrown in WorkRepository.GetActiveSessions. Input parameters: {InputParams}", JsonConvert.SerializeObject(new { WorkID }));
                throw;
            }
        }

        public async Task<bool> UpdateSessionActivityAsync(string connectionID, CancellationToken cancellationToken)
        {
            try
            {
                using var connection = await _dbConnectionFactory.GetOpenConnection(cancellationToken);
                var query = "UPDATE ActiveSessions SET LastActivity = GETDATE() WHERE ConnectionID = @ConnectionID";

                var parameters = new DynamicParameters();
                parameters.Add("@ConnectionID", connectionID);

                var result = await connection.ExecuteAsync(query, parameters, commandType: System.Data.CommandType.Text);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error thrown in WorkRepository.UpdateSessionActivityAsync. Input parameters: {InputParams}", JsonConvert.SerializeObject(new { connectionID }));
                throw;
            }
        }

        public async Task<WorkDetailDTO> GetWorkDetail(Guid WorkID, CancellationToken cancellationToken)
        {
            try
            {
                using var connection = await _dbConnectionFactory.GetOpenConnection(cancellationToken);
                var query = @"
                            SELECT 
                                w.*,
                                u.Username AS CreatorUsername
                            FROM Work w
                            INNER JOIN [User] u ON w.CreatedBy = u.UserID
                            WHERE w.WorkID = @WorkID
                            ";
                var parameters = new DynamicParameters();
                parameters.Add("@WorkID", WorkID);

                var work = await connection.QuerySingleOrDefaultAsync<WorkDetailDTO>(query, parameters, commandType: System.Data.CommandType.Text);

                if (work == null) return null;

                var activeUsersQuery = @"
                                        SELECT UserID, Username, ConnectionID
                                        FROM ActiveSessions 
                                        WHERE WorkID = @WorkID
                                       ";

                work.ActiveUsers = (await connection.QueryAsync<ActiveUserDTO>(activeUsersQuery, parameters, commandType: System.Data.CommandType.Text)).ToList();
                return work;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error thrown in WorkRepository.GetWorkDetail. Input parameters: {InputParams}", JsonConvert.SerializeObject(new { WorkID }));
                throw;
            }
        }

    }
}
