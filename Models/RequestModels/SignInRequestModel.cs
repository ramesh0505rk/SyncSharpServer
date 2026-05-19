using System.ComponentModel.DataAnnotations;

namespace SyncSharpServer.Models.RequestModels
{
    public class SignInRequestModel
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
