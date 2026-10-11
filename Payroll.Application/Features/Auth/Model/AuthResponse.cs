namespace Payroll.Application.Features.Auth.Model
{
    public class AuthResponse
    {
        public required string AccessToken { get; set; }
        public DateTime AccessTokenExpiresAt { get; set; }
        public required string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiresAt { get; set; }
        public required string UserName { get; set; }
        public required string DisplayName { get; set; }
        public required List<string> Roles { get; set; }
    }
}
