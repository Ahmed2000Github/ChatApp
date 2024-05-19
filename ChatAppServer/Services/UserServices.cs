using AutoMapper;
using ChatAppCore.DTOs;
using ChatAppCore.Entities;
using ChatAppServer.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ChatAppServer.Services
{
    public class UserServices : IUserServices
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly IFileManagerServices _fileManagerServices;
        private readonly IMapper _mapper;


        public UserServices(
            IConfiguration configuration,
            UserManager<User> userManager,
            IMapper mapper,
            IFileManagerServices fileManagerServices)
        {
            _configuration = configuration;
            _userManager = userManager;
            _mapper = mapper;
            _fileManagerServices = fileManagerServices;
        }





        public async Task<(LoginResponseDTO, string)> Loging(LoginCredentialsDTO loginCredentials)
        {
            var user = await _userManager.FindByEmailAsync(loginCredentials.Email);
            if (user is null)
            {
                return (null, "user with this email does not exist.");
            }
            if (await _userManager.CheckPasswordAsync(user, loginCredentials.Password))
            {
                var authData = CreateToken(user);
                return (authData, "");
            }
            return (null, "incorrect user password.");

        }



        public async Task<(LoginResponseDTO?, string)> Register(UserDTO parameters)
        {
            var avatarPath = "";
            if (parameters.AvatarImage is not null)
            {
             avatarPath = await _fileManagerServices.UploadFile(parameters.AvatarImage, "avatars");
            }
            await Console.Out.WriteLineAsync("############################# "+parameters.Email.Split('@').First());
            var _user = new User
            {
                FirstName = parameters.FirstName,
                LastName = parameters.LastName,
                Avatar = avatarPath,
                Email = parameters.Email,
                UserName = parameters.Email.Split('@').First(),
            };

            var result = await _userManager.CreateAsync(_user, parameters.Password);

            if (result.Succeeded)
            {
                var authData = CreateToken(_user);
                return (authData, "");
            }
            else
            {
                return (null, result.Errors?.FirstOrDefault()?.Description ?? "User is not created try later.");
            }
        }
        public async Task<(LoginResponseDTO?, string)> RefreshToken(string token)
        {
            var result = ValidateTokenAndGetIdentifier(token);
            if (result.isValid == false || result.nameIdentifier is null)
            {
                return (null, "Invalid refresh token");
            }
            var user = await _userManager.FindByIdAsync(result.nameIdentifier);
            if (user is null)
            {
                return (null, "user with this id does not exist.");
            }

            var authData = CreateToken(user);
            return (authData, "");
           
        }
        private LoginResponseDTO CreateToken(User user)
        {
            List<Claim> accessTokenClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name,user.FirstName+" "+user.LastName),
                new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim(ClaimTypes.Uri,user.Avatar),
                new Claim(ClaimTypes.Role, "USER")
            };
            List<Claim> refreshTokenClaims = new List<Claim>
            {
               new Claim(ClaimTypes.NameIdentifier,user.Id),
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("Jwt:Key").Value));
            var accessTokenExpireIn = DateTime.Now.AddHours(5);
            var refreshTokenExpireIn = DateTime.Now.AddHours(10);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var accessToken = new JwtSecurityToken(
                claims: accessTokenClaims,
                expires: accessTokenExpireIn,
                signingCredentials: creds);
            var refreshToken = new JwtSecurityToken(
                claims: refreshTokenClaims,
                expires: refreshTokenExpireIn,
                signingCredentials: creds);
            var v1 = new JwtSecurityTokenHandler().WriteToken(accessToken);
            var v2 = new JwtSecurityTokenHandler().WriteToken(refreshToken);
            var auth = new LoginResponseDTO
            {
                AccessToken = v1,
                RefreshToken = v2,
                AccessTokenExpireIn = accessTokenExpireIn,
                RefreshTokenExpireIn = refreshTokenExpireIn
            };
            return auth;
        }
        private (bool isValid, string nameIdentifier) ValidateTokenAndGetIdentifier(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                    return (false, null);

                var isValid = jwtToken.ValidTo >= DateTime.UtcNow;

                var nameIdentifierClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
                var nameIdentifier = nameIdentifierClaim?.Value;

                return (isValid, nameIdentifier);
            }
            catch (Exception)
            {
                return (false, null);
            }
        }
    }
}
