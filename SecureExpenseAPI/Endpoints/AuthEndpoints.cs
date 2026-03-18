
using SecureExpenseAPI.Data;
using SecureExpenseAPI.Services.Auth;
using SecureExpenseAPI.Entities;
using Microsoft.EntityFrameworkCore;
using SecureExpenseAPI.DTOs.Auth;
using SecureExpenseAPI.Utils;
using System.Security.Claims;


namespace SecureExpenseAPI.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var authGroup = app.MapGroup("/auth");

       authGroup.MapPost("/register", async (RegisterRequest request, IPasswordHasher passwordHasher, AppDbContext dbContext) =>
        {
            // Complete registration validation including database checks
            var validationResult = await RegistrationValidationUtils.ValidateRegistrationAsync(request.Email, request.Password, dbContext);
            if (!validationResult.IsValid)
            {
                return Results.BadRequest(new { message = validationResult.ErrorMessage });
            }

            var user = new User
            {
                Email = request.Email,
                PasswordHash = passwordHasher.HashPassword(request.Password)
            };

            dbContext.Users.Add(user);
            _ = await dbContext.SaveChangesAsync();


            return Results.Created($"/auth/register/{user.Id}",new RegisterResponse
            {
                Id = user.Id,
                Email = user.Email, 
                Role = user.Role
            });
        });


       authGroup.MapPost("/login", async (LoginRequest request, IPasswordHasher passwordHasher, AppDbContext dbContext, IJwtTokenService jwtTokenService) =>
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());
            if (user == null || !passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                return Results.Unauthorized();
            }

            var token = jwtTokenService.GenerateToken(user);

            return Results.Ok(new LoginResponse { AccessToken = token });
        });

        authGroup.MapPost("/me",async (ClaimsPrincipal userClaims, AppDbContext dbContext) =>
        {
            var userIdClaim = userClaims.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Results.Unauthorized();
            }

            var user = await dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return Results.Unauthorized();
            }

            return Results.Ok(new MeResponse
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role
            });
        }).RequireAuthorization();
    }
}