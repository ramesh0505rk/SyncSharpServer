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

        [HttpDelete("DeleteWork/{WorkID}")]
        public async Task<IActionResult> DeleteWork(Guid WorkID, CancellationToken cancellationToken)
        {
            return Ok(await _workService.DeleteWork(WorkID, cancellationToken));
        }

        [HttpGet("{workID}/versions")]
        public async Task<IActionResult> GetWorkVersions(Guid workID, [FromQuery] int limit, CancellationToken cancellationToken)
        {
            return Ok(await _workService.GetWorkVersions(workID, limit, cancellationToken));
        }

        [HttpPost("AddWorkMember")]
        public async Task<IActionResult> AddWorkMember(AddDeleteWorkMemberRequestModel request, CancellationToken cancellationToken)
        {
            return Ok(await _workService.AddWorkMember(request, cancellationToken));
        }

        [HttpDelete("DeleteWorkMember")]
        public async Task<IActionResult> DeleteWorkMember(AddDeleteWorkMemberRequestModel request, CancellationToken cancellationToken)
        {
            return Ok(await _workService.DeleteWorkMember(request, cancellationToken));
        }

        [HttpGet("{WorkID}/Members")]
        public async Task<IActionResult> GetMembers(Guid WorkID, CancellationToken cancellationToken)
        {
            return Ok(await _workService.GetMembers(WorkID, cancellationToken));
        }

        [HttpGet("{WorkID}/active-sessions")]
        public async Task<IActionResult> GetActiveSessions(Guid WorkID, CancellationToken cancellationToken)
        {
            return Ok(await _workService.GetActiveSessions(WorkID, cancellationToken));
        }

        [HttpGet("{WorkID}/detail")]
        public async Task<IActionResult> GetWorkDetail(Guid WorkID, CancellationToken cancellationToken)
        {
            return Ok(await _workService.GetWorkDetail(WorkID, cancellationToken));
        }
    }
}
