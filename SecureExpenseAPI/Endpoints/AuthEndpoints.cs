using System.Security.Claims;
using SecureExpenseAPI.DTOs.Auth;
using SecureExpenseAPI.Services.Auth;
using SecureExpenseAPI.Utils;

namespace SecureExpenseAPI.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var authGroup = app.MapGroup("/auth");

        authGroup.MapPost("/register", async (RegisterRequest request, IAuthService authService) =>
        {
            var result = await authService.RegisterAsync(request);
            if (result.ErrorMessage != null)
            {
                return Results.BadRequest(new { message = result.ErrorMessage });
            }

            return Results.Ok(result.Data);
        });

        authGroup.MapPost("/login", async (LoginRequest request, IAuthService authService) =>
        {
            var result = await authService.LoginAsync(request);
            if (result.ErrorMessage != null)
            {
                return Results.Unauthorized(); 
            }

            return Results.Ok(result.Data);
        });

        authGroup.MapGet("/me", async (ClaimsPrincipal user, IAuthService authService) =>
        {
            var userId = UserUtils.GetUserIdFromClaims(user);
            
            var me = await authService.GetMeAsync(userId);
            if (me == null)
            {
                return Results.Unauthorized();
            }

            return Results.Ok(me);
        }).RequireAuthorization();
    }
}