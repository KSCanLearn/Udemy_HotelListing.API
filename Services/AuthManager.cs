using AutoMapper;
using HotelListing.API.Common;
using HotelListing.API.Contracts;
using HotelListing.API.Data;
using HotelListing.API.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HotelListing.API.Services
{
    public class AuthManager : IAuthManager
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApiUser> _userManager;
        private readonly IConfiguration _configuration;
        private ApiUser? _apiUser;

        public AuthManager(IMapper mapper, UserManager<ApiUser> userManager, IConfiguration configuration)
        {
            _mapper = mapper;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<string> CreateRefreshToken()
        {
            await _userManager.RemoveAuthenticationTokenAsync(_apiUser, Constants.LOGIN_PROVIDER, Constants.REFRESH_TOKEN);
            var newRefreshToken = await _userManager.GenerateUserTokenAsync(_apiUser, Constants.LOGIN_PROVIDER, Constants.REFRESH_TOKEN);

            await _userManager.SetAuthenticationTokenAsync(_apiUser, Constants.LOGIN_PROVIDER, Constants.REFRESH_TOKEN, newRefreshToken);

            return newRefreshToken;
        }

        public async Task<AuthResponseDto?> Login(LoginDto loginDto)
        {
            _apiUser = await _userManager.FindByEmailAsync(loginDto.Email);
            if (_apiUser == null)
            {
                return null;
            }

            bool isValidUser = await _userManager.CheckPasswordAsync(_apiUser, loginDto.Password);
            if (!isValidUser)
            {
                return null;
            }

            var token = await GenerateToken();
            return new AuthResponseDto
            {
                UserId = _apiUser.Id,
                Token = token,
                RefreshToken = await CreateRefreshToken()
            };
        }

        public async Task<IEnumerable<IdentityError>> Register(ApiUserDto userDto)
        {
            _apiUser = _mapper.Map<ApiUser>(userDto);
            _apiUser.UserName = userDto.Email;

            var result = await _userManager.CreateAsync(_apiUser, userDto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(_apiUser, "User");
            }

            return result.Errors;
        }

        public async Task<AuthResponseDto?> VerifyRefreshToken(AuthResponseDto request)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var tokenContent = jwtSecurityTokenHandler.ReadJwtToken(request.Token);
            var userName = tokenContent.Claims.First(q => q.Type == JwtRegisteredClaimNames.Email).Value;
            _apiUser = await _userManager.FindByNameAsync(userName);

            if (_apiUser == null || _apiUser.Id != request.UserId)
            {
                return null;
            }

            var isValidrefreshToken = await _userManager.VerifyUserTokenAsync(_apiUser, Constants.LOGIN_PROVIDER, Constants.REFRESH_TOKEN, request.RefreshToken);
            if (isValidrefreshToken)
            {
                var token = await GenerateToken();
                return new AuthResponseDto
                {
                    Token = token,
                    UserId = _apiUser.Id,
                    RefreshToken = await CreateRefreshToken()
                };
            }

            await _userManager.UpdateSecurityStampAsync(_apiUser);
            return null;
        }

        private async Task<string> GenerateToken()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var roles = await _userManager.GetRolesAsync(_apiUser);
            var roleClaims = roles.Select(r => new Claim(ClaimTypes.Role, r)).ToList();
            var userClaims = await _userManager.GetClaimsAsync(_apiUser);

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, _apiUser.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, _apiUser.Email),
                new Claim("uid", _apiUser.Id),
            }
            .Union(userClaims).Union(roleClaims);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["JwtSettings:DurationInMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
