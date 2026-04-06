using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SyncSharpServer.Models.RequestModels;

namespace SyncSharpServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn([FromBody] SignInRequestModel request)
        {
            try
            {

                return BadRequest(new { Errors = "Invalid Params" });
            }
            catch (Exception ex)
            {
                throw new Exception($"Invalid Params {ex}", ex);
            }
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequestModel request)
        {
            return Ok();
        }
    }
}
