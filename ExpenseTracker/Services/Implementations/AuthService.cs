using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ExpenseTracker.DTOS.Auth;
using ExpenseTracker.DTOS.Common;
using ExpenseTracker.Entities;
using ExpenseTracker.Services.Helpers;
using ExpenseTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace ExpenseTracker.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IActivityLogService _activityLogService;

        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration, ILogger<AuthService> logger, IHttpContextAccessor httpContextAccessor, IActivityLogService activityLogService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _logger = logger;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
            _activityLogService = activityLogService;
        }

        public Guid? GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }
            _logger.LogWarning("Could not find or parse user ID from claims.");
            return null;
        }

        public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    _logger.LogWarning("Login failed for email {Email}: User not found.", request.Email);
                    await _activityLogService.LogActivityAsync(null, $"Login failed: User not found with email {request.Email}.");
                    return ApiResponse<LoginResponse>.FailureResponse("Invalid email or password.");
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
                if (!result.Succeeded)
                {
                    _logger.LogWarning("Login failed for user {UserId}: Invalid password. Lockout status: {IsLockedOut}", user.Id, result.IsLockedOut);
                    await _activityLogService.LogActivityAsync(user.Id, "Login failed: Invalid password.");
                    return ApiResponse<LoginResponse>.FailureResponse("Invalid email or password.");
                }

                var roles = await _userManager.GetRolesAsync(user);
                var token = GenerateJwtToken(user, roles);
                var loginResponse = new LoginResponse { Token = token };
                await _activityLogService.LogActivityAsync(user.Id, "User logged in successfully.");

                return ApiResponse<LoginResponse>.SuccessResponse(loginResponse, "Login successful.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the login process for email {Email}.", request.Email);
                return ApiResponse<LoginResponse>.FailureResponse(StaticMessages.DefaultErrorMessage);
            }
        }

        private string GenerateJwtToken(User user, IList<string> roles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Secret"]!);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email!),
                new(ClaimTypes.Name, user.FullName)
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}