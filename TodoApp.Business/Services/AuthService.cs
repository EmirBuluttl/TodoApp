using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TodoApp.Business.DTOs;
using TodoApp.Entities;
using TodoApp.Core.Repositories;

namespace TodoApp.Business.Services;

public class AuthService : IAuthService
{
    private readonly IGenericRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public AuthService(IGenericRepository<User> userRepository, IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    public async Task<AuthResultDto> RegisterAsync(RegisterDto dto)
    {
        if (await _userRepository.AnyAsync(u => u.Username == dto.Username || u.Email == dto.Email))
            throw new Exception("User already exists.");

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        await _userRepository.AddAsync(user);
        await _unitOfWork.CommitAsync();

        var token = GenerateJwtToken(user);
        return new AuthResultDto(token, user.Username, user.Email);
    }

    public async Task<AuthResultDto> LoginAsync(LoginDto dto)
    {
        var userQuery = _userRepository.Where(u => u.Username == dto.Username);
        var user = await userQuery.FirstOrDefaultAsync();

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new Exception("Invalid username or password.");

        var token = GenerateJwtToken(user);
        return new AuthResultDto(token, user.Username, user.Email);
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["Secret"] ?? "SuperSecretKeyForTodoAppNeedToBeLongEnough123!";
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"] ?? "TodoApp",
            audience: jwtSettings["Audience"] ?? "TodoAppUsers",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
