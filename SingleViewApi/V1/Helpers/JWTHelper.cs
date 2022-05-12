using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace SingleViewApi.V1.Helpers;

public class JwtHelper
{
    public JigsawCredentials DecodeJigsawCredentials(string jwt)
    {
        //get key from env variable
        var handler = new JwtSecurityTokenHandler();
        var mySecret = "super secret";
        byte[] keybytes = Encoding.ASCII.GetBytes(mySecret);
        SecurityKey securityKey = new SymmetricSecurityKey(keybytes);
        SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.Sha256);
        var validationTokenParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKey = signingCredentials.Key
        };
        handler.ValidateToken(jwt, validationTokenParameters, out SecurityToken validatedToken);

        return GetCredentials(validatedToken.ToString());


    }

    public JigsawCredentials GetCredentials(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
        var results = new JigsawCredentials()
        {
            Username = securityToken.Claims.First(claim => claim.Type == "username").Value,
            Password = securityToken.Claims.First(claim => claim.Type == "password").Value,
        };

        return results;
    }
}

public class JigsawCredentials
{
    public string Username { get; set; }
    public string Password { get; set; }
}



