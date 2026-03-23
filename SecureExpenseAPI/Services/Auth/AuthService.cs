using Microsoft.EntityFrameworkCore;
using SecureExpenseAPI.Data;
using SecureExpenseAPI.DTOs.Auth;
using SecureExpenseAPI.Entities;
using SecureExpenseAPI.Utils;

namespace SecureExpenseAPI.Services.Auth;

public class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(AppDbContext dbContext, IPasswordHasher passwordHasher, IJwtTokenService jwtTokenService)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<(RegisterResponse? Data, string? ErrorMessage)> RegisterAsync(RegisterRequest request)
    {
        var validationResult = await RegistrationValidationUtils.ValidateRegistrationAsync(request.Email, request.Password, _dbContext);
        if (!validationResult.IsValid)
        {
            return (null, validationResult.ErrorMessage);
        }

        var user = new User
        {
            Email = request.Email,
            PasswordHash = _passwordHasher.HashPassword(request.Password)
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        return (new RegisterResponse
        {
            Id = user.Id,
            Email = user.Email,
            Role = user.Role
        }, null);
    }

    public async Task<(LoginResponse? Data, string? ErrorMessage)> LoginAsync(LoginRequest request)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());
        if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return (null, "Invalid email or password.");
        }

        var token = _jwtTokenService.GenerateToken(user);
        return (new LoginResponse { AccessToken = token }, null);
    }

    public async Task<MeResponse?> GetMeAsync(int userId)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null) return null;

        return new MeResponse
        {
            Id = user.Id,
            Email = user.Email,
            Role = user.Role
        };
    }
}
