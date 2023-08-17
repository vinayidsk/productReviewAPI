﻿using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ProductReviewAPI.Models;
namespace ProductReviewAPI.JwtHelpers
{
    public static class JwtHelpers
    {
        //Token validity in days
        private static readonly double validityDays = 1;
        public static IEnumerable<Claim> GetClaims(this UserTokens userAccounts, Guid Id)
        {
            IEnumerable<Claim> claims = new Claim[] {
                new Claim("Id", userAccounts.UserId.ToString()),
                    new Claim(ClaimTypes.PrimarySid, userAccounts.Username),
                    new Claim(ClaimTypes.Role, ((int)userAccounts.Role).ToString()),
                    new Claim(ClaimTypes.NameIdentifier, Id.ToString()),
                    new Claim(ClaimTypes.Expiration, DateTime.UtcNow.AddDays(validityDays).ToString("MMM ddd dd yyyy HH:mm:ss tt"))
            };
            return claims;
        }
        public static IEnumerable<Claim> GetClaims(this UserTokens userAccounts, out Guid Id)
        {
            Id = Guid.NewGuid();
            return GetClaims(userAccounts, Id);
        }
        public static UserTokens GenTokenkey(UserTokens model, JwtSettings jwtSettings)
        {
            try
            {
                var UserToken = new UserTokens();
                if (model == null) throw new ArgumentException(nameof(model));
                // Get secret key
                var key = System.Text.Encoding.ASCII.GetBytes(jwtSettings.IssuerSigningKey);
                Guid Id = Guid.Empty;
                DateTime expireTime = DateTime.UtcNow.AddDays(validityDays);
                UserToken.Validaty = (expireTime - DateTime.UtcNow);
                UserToken.ExpiredTime = expireTime;
                var JWToken = new JwtSecurityToken(issuer: jwtSettings.ValidIssuer, audience: jwtSettings.ValidAudience, claims: GetClaims(model, out Id), notBefore: new DateTimeOffset(DateTime.Now).DateTime, expires: new DateTimeOffset(expireTime).DateTime, signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256));
                UserToken.Token = new JwtSecurityTokenHandler().WriteToken(JWToken);
                UserToken.Username = model.Username;
                UserToken.UserId = model.UserId;
                UserToken.Role = model.Role;
                UserToken.GuidId = Id;
                return UserToken;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
