using System;
using System.Text;
using System.Security.Claims;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Domain.Settings;
using Infrastructure.Identity.Models;
using System.Text.Json;
using System.Linq;
using Application.Constants;

namespace Infrastructure.Identity.Helpers
{
    public interface IJwtHelper
    {
        public ModelAccessToken GenerateToken(IEnumerable<Claim> claims, ModelUser user, string browser, string ipAddress);

        public IEnumerable<Claim> GetClaimsFromJwt(string jwt);
    }

    public class JwtHelper : IJwtHelper
    {
        private readonly JWTSettings _jwtSettings;

        public JwtHelper(IOptions<JWTSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        /// <summary>
        /// Генерируем токен доступа
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        public ModelAccessToken GenerateToken(IEnumerable<Claim> claims, ModelUser user, string browser, string ipAddress)
        {
            var expires = DateTime.Now.AddMinutes(_jwtSettings.DurationInMinutes); // Время жизни токена 1 час

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: GetSigningCredentials());

            var tokenHandler = new JwtSecurityTokenHandler();

            var encryptedToken = tokenHandler.WriteToken(token);

            return new ModelAccessToken(user)
            {
                Token = encryptedToken,
                Created = DateTime.Now,
                Expires = expires,
                CreatedByBrowser = browser,
                CreatedByIp = ipAddress,
                IsOutDated = false,
                IsActive = true
            };
        }

        /// <summary>
        /// Получаем учетные данные для подписи
        /// </summary>
        /// <returns>Учетные данные для подписи</returns>
        private SigningCredentials GetSigningCredentials()
        {
            var secret = Encoding.UTF8.GetBytes(_jwtSettings.Key);

            return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
        }

        public IEnumerable<Claim> GetClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            if (keyValuePairs != null)
            {
                // Роли
                keyValuePairs.TryGetValue(ClaimTypes.Role, out var roles);

                if (roles != null)
                {
                    if (roles.ToString().Trim().StartsWith("["))
                    {
                        var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString());

                        claims.AddRange(parsedRoles.Select(role => new Claim(ClaimTypes.Role, role)));
                    }
                    else
                    {
                        claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
                    }

                    keyValuePairs.Remove(ClaimTypes.Role);
                }

                // Права доступа
                keyValuePairs.TryGetValue(AllPermissionTypes.Permission, out var permissions);

                if (permissions != null)
                {
                    if (permissions.ToString().Trim().StartsWith("["))
                    {
                        var parsedPermissions = JsonSerializer.Deserialize<string[]>(permissions.ToString());
                        claims.AddRange(parsedPermissions.Select(permission => new Claim(AllPermissionTypes.Permission, permission)));
                    }
                    else
                    {
                        claims.Add(new Claim(AllPermissionTypes.Permission, permissions.ToString()));
                    }
                    keyValuePairs.Remove(AllPermissionTypes.Permission);
                }

                claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));

                // Сервисы
                keyValuePairs.TryGetValue(AllPermissionTypes.Service, out var services);

                if (services != null)
                {
                    if (services.ToString().Trim().StartsWith("["))
                    {
                        var parsedServices = JsonSerializer.Deserialize<string[]>(services.ToString());
                        claims.AddRange(parsedServices.Select(permission => new Claim(AllPermissionTypes.Service, permission)));
                    }
                    else
                    {
                        claims.Add(new Claim(AllPermissionTypes.Service, services.ToString()));
                    }

                    keyValuePairs.Remove(AllPermissionTypes.Service);
                }

                claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));
            }

            return claims;
        }

        private static byte[] ParseBase64WithoutPadding(string payload)
        {
            payload = payload.Trim().Replace('-', '+').Replace('_', '/');
            var base64 = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
            return Convert.FromBase64String(base64);
        }
    }
}
