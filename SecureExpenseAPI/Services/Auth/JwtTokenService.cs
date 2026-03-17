using SecureExpenseAPI.Entities;

namespace SecureExpenseAPI.Services.Auth;

public class JwtTokenService : IJwtTokenService
{
    public string GenerateToken(User user)
    {
        // Implement JWT token generation logic here
        // This is a placeholder implementation and should be replaced with actual JWT generation code
        return "dummy-jwt-token";
    }
}