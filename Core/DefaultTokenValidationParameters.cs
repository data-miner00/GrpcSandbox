using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace GrpcSandbox.Core;

public sealed class DefaultTokenValidationParameters : TokenValidationParameters
{
    public DefaultTokenValidationParameters(string issuer, string audience, string key)
    {
        this.ValidIssuer = issuer;
        this.ValidAudience = audience;
        this.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    }
}
