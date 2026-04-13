using System.ComponentModel.DataAnnotations;

namespace SyncSharpServer.Models.RequestModels
{
    public class SignInRequestModel
    {
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;
    }
}
