using Microsoft.AspNetCore.Mvc;
using SyncSharpServer.Interfaces;
using SyncSharpServer.Models.RequestModels;

namespace SyncSharpServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService _service;
        public UserController(IUserService service)
        {
            _service = service;
        }

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn([FromBody] SignInRequestModel request, CancellationToken cancellationToken)
        {
            return Ok(await _service.SignIn(request, cancellationToken));
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequestModel request, CancellationToken cancellationToken)
        {
            return Ok(await _service.SignUp(request, cancellationToken));
        }
    }
}
