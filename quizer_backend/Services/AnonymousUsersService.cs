using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace quizer_backend.Services {
    public static class AnonymousUsersService {
        private const string Key = "4A404E635266556A586E5A7234753778214125442A472D4B6150645367566B59703373357638792F423F4528482B4D6251655468576D5A7134743777397A2443";
        private const string Audience = "https://quizer.azurewebsites.net";
        private const string Issuer = "https://quizer.azurewebsites.net";

        public static string GenerateTokenFromUserId(string userId) {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var secToken = new JwtSecurityToken(
                signingCredentials: credentials,
                issuer: Issuer,
                audience: Audience,
                claims: new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, userId)
                });

            var handler = new JwtSecurityTokenHandler();
            return handler.WriteToken(secToken);
        }

        public static string GetUserId(string token) {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetValidationParameters();

            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            var guidString = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            return guidString;
        }

        private static TokenValidationParameters GetValidationParameters() {
            return new TokenValidationParameters {
                ValidateLifetime = false, // Because there is no expiration in the generated token
                ValidateAudience = true, // Because there is no audiance in the generated token
                ValidateIssuer = true,   // Because there is no issuer in the generated token
                ValidIssuer = Issuer,
                ValidAudience = Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key)) // The same key as the one that generate the token
            };
        }
    }
}
