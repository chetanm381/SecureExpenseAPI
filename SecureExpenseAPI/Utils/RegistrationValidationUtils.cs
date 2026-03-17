using System.Text.RegularExpressions;
using SecureExpenseAPI.Data;
using Microsoft.EntityFrameworkCore;


namespace SecureExpenseAPI.Utils;

public static class RegistrationValidationUtils
{
    // Email validation using RFC 5322 compliant regex
    private static readonly Regex EmailRegex = new Regex(
        @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    // Password validation regex - requires at least 8 chars, 1 uppercase, 1 lowercase, 1 digit, 1 special char
    private static readonly Regex PasswordRegex = new Regex(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        RegexOptions.Compiled);

    /// <summary>
    /// Validates an email address using RFC 5322 compliant regex
    /// </summary>
    public static ValidationResult ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return ValidationResult.Failure("Email address is required");
        }

        if (email.Length > 254) // RFC 5321 limit
        {
            return ValidationResult.Failure("Email address is too long");
        }

        if (!EmailRegex.IsMatch(email))
        {
            return ValidationResult.Failure("Invalid email address format");
        }

        return ValidationResult.Success();
    }

    /// <summary>
    /// Validates a password with strong requirements
    /// </summary>
    public static ValidationResult ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return ValidationResult.Failure("Password is required");
        }

        if (password.Length < 8)
        {
            return ValidationResult.Failure("Password must be at least 8 characters long");
        }

        if (password.Length > 128)
        {
            return ValidationResult.Failure("Password must not exceed 128 characters");
        }

        if (!PasswordRegex.IsMatch(password))
        {
            return ValidationResult.Failure("Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character (@$!%*?&)");
        }

        return ValidationResult.Success();
    }

    /// <summary>
    /// Validates both email and password
    /// </summary>
    public static ValidationResult ValidateCredentials(string email, string password)
    {
        var emailResult = ValidateEmail(email);
        if (!emailResult.IsValid)
        {
            return emailResult;
        }

        var passwordResult = ValidatePassword(password);
        if (!passwordResult.IsValid)
        {
            return passwordResult;
        }

        return ValidationResult.Success();
    }

    /// <summary>
    /// Validates if email is already in use by checking the database
    /// </summary>
    public static async Task<ValidationResult> ValidateEmailUniquenessAsync(string email, AppDbContext dbContext)
    {
        if (await dbContext.Users.AnyAsync(u => u.Email == email))
        {
            return ValidationResult.Failure("Email already in use");
        }

        return ValidationResult.Success();
    }

    /// <summary>
    /// Performs complete registration validation including database checks
    /// </summary>
    public static async Task<ValidationResult> ValidateRegistrationAsync(string email, string password, AppDbContext dbContext)
    {
        // First validate format
        var formatResult = ValidateCredentials(email, password);
        if (!formatResult.IsValid)
        {
            return formatResult;
        }

        // Then check database uniqueness
        var uniquenessResult = await ValidateEmailUniquenessAsync(email, dbContext);
        if (!uniquenessResult.IsValid)
        {
            return uniquenessResult;
        }

        return ValidationResult.Success();
    }
}

/// <summary>
/// Represents the result of a validation operation
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; private set; }
    public string? ErrorMessage { get; private set; }

    private ValidationResult(bool isValid, string? errorMessage = null)
    {
        IsValid = isValid;
        ErrorMessage = errorMessage;
    }

    public static ValidationResult Success() => new ValidationResult(true);
    public static ValidationResult Failure(string message) => new ValidationResult(false, message);
}