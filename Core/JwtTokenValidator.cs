namespace GrpcSandbox.Core;

using Google.Protobuf.WellKnownTypes;
using GrpcSandbox.Core.Protos;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public sealed class JwtTokenValidator
{
    private readonly UserManager<IdentityUser>? userManager;
    private readonly SignInManager<IdentityUser>? signInManager;
    private readonly string issuer;
    private readonly string key;
    private readonly string audience;

    public JwtTokenValidator(
        string audience,
        string issuer,
        string key)
    {
        this.audience = audience;
        this.issuer = issuer;
        this.key = key;
    }

    public async Task<TokenResponse> GenerateTokenWithProviderAsync(TokenRequest request)
    {
        var user = await this.userManager.FindByNameAsync(request.Username);
        var result = new TokenResponse
        {
            Success = false,
        };

        var hardcodeUsername = "shaun";
        var isAdmin = request.Username.Equals(hardcodeUsername, StringComparison.OrdinalIgnoreCase);

        if (user is not null || isAdmin)
        {
            user ??= new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = "shaun@email.com",
                UserName = request.Username,
            };

            var check = await this.signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (check.Succeeded || isAdmin)
            {
                List<Claim> claims = [
                    new(JwtRegisteredClaimNames.Sub, user.Email),
                    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new(JwtRegisteredClaimNames.UniqueName, user.UserName),
                ];

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.key));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    this.issuer,
                    this.audience,
                    claims,
                    expires: DateTime.Now.AddMinutes(10),
                    signingCredentials: creds);

                result.Token = new JwtSecurityTokenHandler().WriteToken(token);
                result.Expiration = token.ValidTo.ToTimestamp();
                result.Success = true;
            }
        }

        return result;
    }

    public async Task<TokenResponse> GenerateTokenAsync(TokenRequest request)
    {
        var result = new TokenResponse
        {
            Success = false,
        };

        var hardcodeUsername = "shaun";
        var isAdmin = request.Username.Equals(hardcodeUsername, StringComparison.OrdinalIgnoreCase);

        var user = new IdentityUser
        {
            Id = Guid.NewGuid().ToString(),
            Email = "shaun@email.com",
            UserName = request.Username,
        };

        if (isAdmin)
        {
            List<Claim> claims = [
                new(JwtRegisteredClaimNames.Sub, user.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.UniqueName, user.UserName),
            ];

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                this.issuer,
                this.audience,
                claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds);

            try
            {
                result.Token = new JwtSecurityTokenHandler().WriteToken(token);
                result.Expiration = token.ValidTo.ToTimestamp();
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Token = string.Empty;
                result.Expiration = DateTime.MinValue.ToTimestamp();
                result.Success = false;
            }
        }

        return result;
    }
}
