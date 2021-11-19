using API_Noface.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web.Http;

namespace API_Noface.Controllers
{
    public class TokenController : ApiController
    {
        private readonly NofaceDbContext db = new NofaceDbContext();

        public string GenerateToken(string idUser)
        {
            string key = "KeyBaoMatSieuCapVoDich";
            var issuer = "http://www.noface.somee.com";
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var permClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString()),
                new Claim("idUser", idUser)
            };

            var token = new JwtSecurityToken(issuer,
                                issuer,
                                permClaims,
                                expires: DateTime.Now.AddHours(2),
                                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefeshToken(string idUser)
        {
            string key = "KeyBaoMatSieuCapVoDich";
            var issuer = "http://www.noface.somee.com";
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var permClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString()),
                new Claim("Exp", DateTime.Now.AddDays(7).ToString()),
                new Claim("idUser", idUser)
            };

            var token = new JwtSecurityToken(issuer,
                                issuer,
                                permClaims,
                                expires: DateTime.Now.AddDays(7),
                                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string refeshToken)
        {
            try
            {
            string key = "KeyBaoMatSieuCapVoDich";
                var keysecret = Encoding.ASCII.GetBytes(key);
                var handler = new JwtSecurityTokenHandler();

                var validations = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(keysecret),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false
                };

                var claims = handler.ValidateToken(refeshToken, validations, out var tokenSecure);
                return claims;
            }
            catch
            {
                throw new SecurityTokenException("Hông phải user mà đòi lấy token");
            }
        }

        [Route("get-token/{idUser}")]
        [HttpPost]
        public IHttpActionResult GetToken(string idUser)
        {
            var user = db.User.FirstOrDefault(u => u.IDUser.Equals(idUser) == true);

            if (user != null)
            {
                var refreshToken = GenerateRefeshToken(idUser);
                var token = GenerateToken(idUser);
                SaveRefeshToken(idUser, refreshToken);
                return Ok(new Token(token, refreshToken));
            }
            else
            {
                return BadRequest("Hông phải user mà đòi lấy token");
            }
        }

        [Route("refresh-token")]
        [HttpPost]
        public IHttpActionResult RefeshToken(Token token)
        {
            var principal = GetPrincipalFromExpiredToken(token.RefreshToken);
            string idUser = principal.Claims.Where(p => p.Type == "idUser").FirstOrDefault()?.Value;

            var saveRefreshToken = GetRefreshToken(idUser);

            if (saveRefreshToken != token.RefreshToken)
            {
                throw new SecurityTokenException("Hông phải user mà đòi lấy token");
            }

            DateTime exp = DateTime.Parse(principal.Claims.Where(p => p.Type == "Exp").FirstOrDefault()?.Value);

            if (DateTime.Now > exp)
            {
                return Ok(new Token("hết hạn", ""));
            }

            var newJwtToken = GenerateToken(idUser);
            return Ok(new Token(newJwtToken, token.RefreshToken));
        }

        public void SaveRefeshToken(string idUser, string refeshToken)
        {
            var userdb = db.User.FirstOrDefault(u => u.IDUser.Equals(idUser) == true);

            if (userdb != null)
            {
                userdb.RefeshToken = refeshToken;
                db.SaveChanges();
            }
        }

        public string GetRefreshToken(string idUser)
        {
            var user = db.User.FirstOrDefault(u => u.IDUser.Equals(idUser) == true);
            if (user != null)
            {
                return user.RefeshToken;
            }
            return null;
        }

    }
}
