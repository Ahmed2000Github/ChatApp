
namespace ChatAppCore.DTOs
{
    public class LoginResponseDTO
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime AccessTokenExpireIn { get; set; }
        public DateTime RefreshTokenExpireIn { get; set; }
    }
}
