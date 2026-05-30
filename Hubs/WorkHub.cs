using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using SyncSharpServer.Interfaces;

namespace SyncSharpServer.Hubs
{
    public class WorkHub : Hub
    {
        private readonly IWorkService _workService;
        private readonly IWorkRepository _workRepository;
        private readonly ILogger _logger;
        public WorkHub(IWorkService workService, IWorkRepository workRepository, ILogger<WorkHub> logger)
        {
            _workService = workService;
            _workRepository = workRepository;
            _logger = logger;
        }

        // ======================== JOIN WORK ========================
        public async Task JoinWork(Guid WorkID, Guid UserID, string username)
        {
            try
            {
                // Check if work exists
                var work = await _workRepository.GetWorkByID(WorkID, Context.ConnectionAborted);
                if (work == null)
                {
                    await Clients.Caller.SendAsync("Error", "Work not found");
                    return;
                }

                // Check if user has access
                var hasAccess = await _workService.HasAccess(WorkID, UserID, Context.ConnectionAborted);
                if (!hasAccess)
                {
                    await Clients.Caller.SendAsync("Error", "Access denied");
                    return;
                }

                // Save active session
                await _workService.SaveActiveSession(WorkID, UserID, Context.ConnectionId, username, Context.ConnectionAborted);

                // Join SignalR group
                string groupName = $"Work-{WorkID}";
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

                // Send current work to the caller
                await Clients.Caller.SendAsync("WorkLoaded", new
                {
                    WorkID = work.WorkID,
                    Title = work.Title,
                    Description = work.Description,
                    Code = work.Code,
                    Language = work.Language,
                    LastModified = work.LastModified
                });

                // Send active users to caller
                var activeSessions = await _workService.GetActiveSessions(WorkID, Context.ConnectionAborted);
                await Clients.Caller.SendAsync("ActiveUsers", activeSessions.Select(s =>
                new
                {
                    s.UserID,
                    s.UserName,
                    s.ConnectionID
                }));

                // Notify others in the group
                await Clients.OthersInGroup(groupName).SendAsync("UserJoined", new
                {
                    UserID,
                    username,
                    ConnectionID = Context.ConnectionId,
                    Timestamp = DateTime.UtcNow
                });

                _logger.LogInformation($"User {username} (ID: {UserID}) joined work {WorkID}");
            }
            catch (Exception ex)
            {
                var inputParams = new { WorkID, UserID, username };
                _logger.LogError(ex, "Error thrown in WorkHub.JoinWork. Input parameters: {InputParams}", JsonConvert.SerializeObject(inputParams));
                await Clients.Caller.SendAsync("Error", "Failed to join work. Please try again later.");
            }
        }

        // ======================== UPDATE CODE - REALTIME ========================
        public async Task UpdateCode(Guid workID, string code, int cursorPosition)
        {
            try
            {
                string groupName = $"Work-{workID}";

                // Update session activity
                await _workService.UpdateSessionActivityAsync(Context.ConnectionId, Context.ConnectionAborted);

                // Broadcast every one in the group except the sender
                await Clients.OthersInGroup(groupName).SendAsync("CodeUpdated", new
                {
                    code,
                    cursorPosition,
                    updatedBy = Context.ConnectionId,
                    timestamp = DateTime.UtcNow
                });

                _logger.LogInformation($"Code updated in work {workID} by connection {Context.ConnectionId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in UpdateCode for WorkID: {workID}");
            }
        }

        // ======================== SAVE Snapshot (Version) ========================
        public async Task SaveSnapshot(Guid workID, string code, Guid userID, string description)
        {
            try
            {
                await _workRepository.CreateVersionAsync(workID, code, userID, description, Context.ConnectionAborted);

                string groupName = $"Work-{workID}";

                await Clients.Group(groupName).SendAsync("SnapshotSaved", new
                {
                    workID,
                    timestamp = DateTime.UtcNow,
                    savedBy = userID
                });

                _logger.LogInformation($"Snapshot saved for work {workID} by User {userID}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in SaveSnapshot for WorkId: {workID}");
            }
        }

        // ======================== CURSOR POSITION ========================
        public async Task UpdateCursor(Guid workID, int cursorPosition, int lineNumber)
        {
            try
            {
                string groupName = $"Work-{workID}";

                // Broadcast the cursor position to others in the group except the caller
                await Clients.OthersInGroup(groupName).SendAsync("CursorMoved", new
                {
                    connectionID = Context.ConnectionId,
                    cursorPosition,
                    lineNumber,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in UpdateCursor for WorkID: {workID}");
            }
        }

        // ======================== LEAVE WORK ========================
        public async Task LeaveWork(Guid workID, Guid userID, string username)
        {
            try
            {
                string groupName = $"Work-{workID}";

                // Remove from SignalR group
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

                // Remove the session from the DB
                await _workService.RemoveActiveSessionAsync(Context.ConnectionId, Context.ConnectionAborted);

                // Notify others in the group
                await Clients.OthersInGroup(groupName).SendAsync("UserLeft", new
                {
                    userID,
                    username,
                    connectionID = Context.ConnectionId,
                    timestamp = DateTime.UtcNow
                });

                _logger.LogInformation($"User {username} (ID: {userID}) left work {workID}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in LeaveWork for WorkID: {workID}");
            }
        }

        // ======================== DISCONNECTED ========================
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                // Find session in the active sessions by Connection ID
                var session = await _workService.GetSessionByConnectionID(Context.ConnectionId, Context.ConnectionAborted);

                if (session != null)
                {
                    string groupName = $"Work-{session.WorkID}";

                    // Notify others in the group
                    await Clients.OthersInGroup(groupName).SendAsync("UserDisconnected", new
                    {
                        userID = session.ConnectionID,
                        username = session.UserName,
                        workID = session.WorkID,
                        timestamp = DateTime.UtcNow
                    });

                    // Remove the session from DB
                    await _workService.RemoveActiveSessionAsync(Context.ConnectionId, Context.ConnectionAborted);

                    _logger.LogInformation($"User {session.UserName} disconnected from work {session.WorkID}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in OnDisconnectedAsync");
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
