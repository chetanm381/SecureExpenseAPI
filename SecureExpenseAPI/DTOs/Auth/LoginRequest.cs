using System.ComponentModel.DataAnnotations;

namespace SecureExpenseAPI.DTOs.Auth;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
}
