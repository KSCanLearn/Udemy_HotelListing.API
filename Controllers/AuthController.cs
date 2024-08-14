using HotelListing.API.Contracts;
using HotelListing.API.Models.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthManager _authManager;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthManager authManager, ILogger<AuthController> logger)
        {
            _authManager = authManager;
            _logger = logger;
        }

        // POST: api/Auth/register
        [HttpPost]
        [Route("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Register([FromBody] ApiUserDto apiUserDto)
        {
            _logger.LogInformation("AuthController - Register - Attempt to Register: {email}", apiUserDto.Email);
            var errors = await _authManager.Register(apiUserDto);
            if (errors.Any())
            {
                _logger.LogError("AuthController - Register - Error to Register: {email}", apiUserDto.Email);
                foreach (var error in errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }
            _logger.LogInformation("AuthController - Register - Success to Register: {email}", apiUserDto.Email);
            return Ok();

        }

        // POST: api/Auth/login
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Login([FromBody] LoginDto apiUserDto)
        {
            var authResponse = await _authManager.Login(apiUserDto);
            if (authResponse == null)
            {
                return Unauthorized();
            }
            return Ok(authResponse);

        }

        // POST: api/Auth/refreshtoken
        [HttpPost]
        [Route("refreshtoken")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> RefreshToken([FromBody] AuthResponseDto authRequest)
        {
            var authResponse = await _authManager.VerifyRefreshToken(authRequest);
            if (authResponse == null)
            {
                return Unauthorized();
            }
            return Ok(authResponse);

        }


    }
}
