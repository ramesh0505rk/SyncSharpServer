using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SyncSharpServer.Interfaces;
using SyncSharpServer.Models.RequestModels;

namespace SyncSharpServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkController : ControllerBase
    {
        private readonly IWorkService _workService;
        public WorkController(IWorkService workService)
        {
            _workService = workService;
        }

        [HttpGet("GetWorkByID/{WorkID}")]
        public async Task<IActionResult> GetWorkByID(Guid WorkID, CancellationToken cancellationToken)
        {
            return Ok(await _workService.GetWorkByID(WorkID, cancellationToken));
        }

        [HttpGet("GetUserWorks/{UserID}")]
        public async Task<IActionResult> GetUserWorks(Guid UserID, CancellationToken cancellationToken)
        {
            return Ok(await _workService.GetUserWorks(UserID, cancellationToken));
        }

        [HttpPost("CreateWork")]
        public async Task<IActionResult> CreateWork(CreateWorkRequestModel request, CancellationToken cancellationToken)
        {
            return Ok(await _workService.CreateWork(request, cancellationToken));
        }

        [HttpPut("UpdateWork")]
        public async Task<IActionResult> UpdateWork(UpdateWorkRequestModel request, CancellationToken cancellationToken)
        {
            return Ok(await _workService.UpdateWork(request, cancellationToken));
        }
    }
}
