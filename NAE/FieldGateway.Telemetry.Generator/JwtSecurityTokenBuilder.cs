

namespace FieldGateway.Telemetry.Generator
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Protocols.WSTrust;
    using System.IdentityModel.Tokens;
    using System.Security.Claims;
    public abstract class JwtSecurityTokenBuilder
    {
        public static string Create(string issuer, string audience, string deviceClaimType, string deviceId, int lifetimeMinutes, string symmetricKey)
        {
            DateTime now = DateTime.UtcNow;
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(deviceClaimType, deviceId));

            JwtSecurityTokenHandler jwt = new JwtSecurityTokenHandler();
            SecurityTokenDescriptor std = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                TokenIssuerName = issuer,
                AppliesToAddress = audience,
                Lifetime = new Lifetime(now, now.AddMinutes(lifetimeMinutes)),
                SigningCredentials = new SigningCredentials(
                    new InMemorySymmetricSecurityKey(Convert.FromBase64String(symmetricKey)),
                    "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256",
                    "http://www.w3.org/2001/04/xmlenc#sha256")
            };

            string tokenString = jwt.WriteToken(jwt.CreateToken(std));

            return tokenString;
        }


        public static string Create(string issuer, string audience, IEnumerable<Claim> claims, int lifetimeMinutes, string symmetricKey)
        {
            DateTime now = DateTime.UtcNow;

            JwtSecurityTokenHandler jwt = new JwtSecurityTokenHandler();
            SecurityTokenDescriptor std = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                TokenIssuerName = issuer,
                AppliesToAddress = audience,
                Lifetime = new Lifetime(now, now.AddMinutes(lifetimeMinutes)),
                SigningCredentials = new SigningCredentials(
                    new InMemorySymmetricSecurityKey(Convert.FromBase64String(symmetricKey)),
                    "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256",
                    "http://www.w3.org/2001/04/xmlenc#sha256")
            };

            string tokenString = jwt.WriteToken(jwt.CreateToken(std));

            return tokenString;
        }
    }
}
