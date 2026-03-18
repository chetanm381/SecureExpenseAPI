
namespace SecureExpenseAPI.DTOs.Auth;

public class MeResponse
{
    public int Id {get;set;}
    public string Email {get;set;}=String.Empty;
    public string Role {get;set;}=String.Empty;
}