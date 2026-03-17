using SecureExpenseAPI.Entities;

namespace SecureExpenseAPI.Services.Auth;

public interface IJwtTokenService
{
    string GenerateToken(User user);
}