using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using SyncSharpServer.Interfaces;

namespace SyncSharpServer.Hubs
{
	public class WorkHub : Hub
	{
		private readonly IWorkService _workService;
		private readonly ILogger _logger;
		public WorkHub(IWorkService workService, ILogger<WorkHub> logger)
		{
			_workService = workService;
			_logger = logger;
		}

		public async Task JoinWork(Guid WorkID, Guid UserID, string username)
		{
			try
			{
				var work = await _workService.GetWorkByID(WorkID, Context.ConnectionAborted);
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
