using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SecureExpenseAPI.Configurations;
using SecureExpenseAPI.Entities;

namespace SecureExpenseAPI.Services.Auth;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _jwtSettings;

    public JwtTokenService(IOptions<JwtSettings> jwtOptions)
    {
        _jwtSettings = jwtOptions.Value;
    }

    public string GenerateToken(User user)
    {

        var SignedKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var SigningCredentials = new SigningCredentials(SignedKey, SecurityAlgorithms.HmacSha256);

        var Claims = new List<Claim>{
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.Role,user.Role.ToString())

    };


        var Token = new JwtSecurityToken(_jwtSettings.Issuer, _jwtSettings.Audience, Claims, expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes), signingCredentials: SigningCredentials);
        return new JwtSecurityTokenHandler().WriteToken(Token);
    }
}