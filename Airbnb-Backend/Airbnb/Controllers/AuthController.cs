using Airbnb.DTOs;
using Airbnb.Models;
using Airbnb.Services;
using Microsoft.AspNetCore.Mvc;


namespace Airbnb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController: ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ILogger<AuthController> _logger;


        public AuthController(AuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }
        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginReq request)
        {
            try
            {
                var user = await _authService.LoginAsync(request.Username, request.Password);
                if (user == null) return Unauthorized(new { err = "Invalid username or password" });

                var token = _authService.GetLoginToken(user);

                _logger.LogInformation("User login: {@user}", user);

                Response.Cookies.Append("loginToken", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None
                });

                return Ok(new { user, token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to login");
                return Unauthorized(new { err = "Failed to login" });
            }
        }

        // POST: api/auth/signup
        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupRequest request)
        {
            try
            {
                var newUser = new User
                {
                    Username = request.Username,
                    Fullname = request.Fullname,
                    Password = request.Password,
                    PolicyNumber = request.PolicyNumber,
                    ImgUrl = request?.ImgUrl
                };

                var account = await _authService.SignupAsync(newUser);

                _logger.LogDebug("New account created: {@account}", account);

                var loggedInUser = await _authService.LoginAsync(request.Username, request.Password);
                if (loggedInUser == null) return Unauthorized(new { err = "Login failed after signup" });

                var token = _authService.GetLoginToken(loggedInUser);
                _logger.LogInformation("User signup: {@user}", loggedInUser);

                Response.Cookies.Append("loginToken", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None
                });

                return Ok(new { user = loggedInUser, token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to signup");

                if (ex.Message.Contains("Username already taken"))
                    return BadRequest(new { err = ex.Message });

                return StatusCode(500, new { err = "Failed to signup" });
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] User user)
        {
            var updatedUser = await _authService.UpdateUserAsync(user);
            return Ok(updatedUser);
        }

        // POST: api/auth/logout
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            try
            {
                Response.Cookies.Delete("loginToken");
                return Ok(new { msg = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to logout");
                return StatusCode(500, new { err = "Failed to logout" });
            }
        }

    }
}
