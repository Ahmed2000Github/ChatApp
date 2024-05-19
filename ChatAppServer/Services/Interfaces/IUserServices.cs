using ChatAppCore.DTOs;

namespace ChatAppServer.Services
{
    public interface IUserServices
    {
        public Task<(LoginResponseDTO, string)> Loging(LoginCredentialsDTO loginCredentials);
        public Task<(LoginResponseDTO, string)> Register(UserDTO user);
        public Task<(LoginResponseDTO, string)> RefreshToken(string token);
    }
}
