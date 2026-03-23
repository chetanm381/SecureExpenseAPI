using SecureExpenseAPI.DTOs.Auth;

namespace SecureExpenseAPI.Services.Auth;

public interface IAuthService
{
    Task<(RegisterResponse? Data, string? ErrorMessage)> RegisterAsync(RegisterRequest request);
    Task<(LoginResponse? Data, string? ErrorMessage)> LoginAsync(LoginRequest request);
    Task<MeResponse?> GetMeAsync(int userId);
}
