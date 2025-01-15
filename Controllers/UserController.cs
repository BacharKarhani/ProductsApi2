using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Product.Models.Interfaces;
using ProductsApi.Models;
using ProductsApi.Models.Dtos;


[Route("api/user")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserRepository userRepository, IConfiguration configuration, ILogger<UserController> logger)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        try
        {
            _logger.LogInformation("Register attempt for username: {Username}", registerDto.Username);

            var existingUser = await _userRepository.GetByUsernameAsync(registerDto.Username);
            if (existingUser != null)
            {
                _logger.LogWarning("Username already exists: {Username}", registerDto.Username);
                return Conflict("Username already exists.");
            }

            var newUser = new User
            {
                Name = registerDto.Name,
                Username = registerDto.Username,
                Email = registerDto.Email,
                Password = registerDto.Password, // Ideally, hash the password before storing
                Role = registerDto.Role
            };

            await _userRepository.AddAsync(newUser);

            _logger.LogInformation("User registered successfully: {Username}", registerDto.Username);

            return CreatedAtAction(nameof(Register), new { username = newUser.Username }, newUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while registering user: {Username}", registerDto.Username);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        try
        {
            _logger.LogInformation("Login attempt for username: {Username}", loginDto.Username);

            // Retrieve the user from the database
            var user = await _userRepository.GetByUsernameAsync(loginDto.Username);

            if (user == null || user.Password != loginDto.Password) // Ideally, compare hashed passwords
            {
                _logger.LogWarning("Invalid login attempt for username: {Username}", loginDto.Username);
                return Unauthorized("Invalid username or password.");
            }

            // Generate the JWT token with the user's role included
            var token = GenerateJwtToken(user);

            _logger.LogInformation("Login successful for username: {Username}", loginDto.Username);
            return Ok(new { token });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while logging in user: {Username}", loginDto.Username);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        try
        {
            _logger.LogInformation("Logout request received.");

            // Expire the authentication token cookie by setting it with an empty value and past expiration date
            Response.Cookies.Append("AuthToken", string.Empty, new CookieOptions
            {
                HttpOnly = true, // Ensure cookie cannot be accessed via JavaScript
                Secure = true,   // Use HTTPS for secure transmission
                Expires = DateTime.UtcNow.AddDays(-1), // Expire the cookie immediately
                SameSite = SameSiteMode.Strict // Enforce strict same-site rules
            });

            _logger.LogInformation("Token removed from cookies successfully.");
            return Ok(new { message = "Logged out successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during logout.");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
