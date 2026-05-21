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
    }
}
