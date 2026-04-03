namespace Azureblob2.Data
{
    public class LoginRequest
    {
        public string UserId { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
}
