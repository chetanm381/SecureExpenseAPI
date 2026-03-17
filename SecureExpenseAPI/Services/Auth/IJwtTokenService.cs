using SecureExpenseAPI.Entities;

namespace SecureExpenseAPI.Services.Auth;

public interface IJwtTokenService
{
    string GenerateJwtToken(User user);
}