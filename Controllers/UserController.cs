using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SyncSharpServer.Interfaces;
using SyncSharpServer.Models.RequestModels;

namespace SyncSharpServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;
        public UserController(IUserService service)
        {
            _service = service;
        }

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn([FromBody] SignInRequestModel request)
        {
            return Ok(await _service.SignIn(request));
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequestModel request)
        {
            return Ok();
        }
    }
}
