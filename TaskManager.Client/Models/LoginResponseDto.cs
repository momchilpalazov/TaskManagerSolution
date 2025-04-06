namespace TaskManager.Client.Models
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = "";
        public string RefreshToken { get; set; } = "";
        public string Email { get; set; } = "";
        public string FullName { get; set; } = "";
    }
}
