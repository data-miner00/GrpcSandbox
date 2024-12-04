namespace GrpcSandbox.Core;

using Microsoft.IdentityModel.Tokens;
using System.Text;

public sealed class DefaultTokenValidationParameters : TokenValidationParameters
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultTokenValidationParameters"/> class.
    /// </summary>
    /// <param name="issuer">The issuer.</param>
    /// <param name="audience">The audience.</param>
    /// <param name="key">The issuer signing key.</param>
    public DefaultTokenValidationParameters(string issuer, string audience, string key)
    {
        this.ValidIssuer = issuer;
        this.ValidAudience = audience;
        this.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    }
}
