using Microsoft.AspNetCore.Identity;
using SecureExpenseAPI.Entities;

namespace SecureExpenseAPI.Services.Auth;

public class PasswordHasher : IPasswordHasher
{
    private readonly IPasswordHasher<User> _passwordHasher;

    public PasswordHasher()
    {
        // Using ASP.NET Core's built-in PasswordHasher specifically typed for our User entity
        _passwordHasher = new PasswordHasher<User>();
    }

    public string HashPassword(string password)
    {
        // The first argument is the user object, which is mostly used if you configure 
        // the hasher to use a user-specific value for the salt. It's safe to pass null 
        // or a default object with the standard V3 password hashing.
        return _passwordHasher.HashPassword(null!, password);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        var result = _passwordHasher.VerifyHashedPassword(null!, hashedPassword, password);
        
        return result == PasswordVerificationResult.Success 
            || result == PasswordVerificationResult.SuccessRehashNeeded;
    }
}