using System.ComponentModel.DataAnnotations;

namespace SecureExpenseAPI.DTOs.Auth;

public class RegisterResponse
{
    public int id {get;set;}
    public string Email {get;set;}=String.Empty;
    public string Role {get;set;}=String.Empty;
}