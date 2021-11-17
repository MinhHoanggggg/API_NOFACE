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
                new Claim("Exp", DateTime.Now.AddSeconds(5).ToString()),
                new Claim("idUser", idUser)
            };

            var token = new JwtSecurityToken(issuer,
                                issuer,
                                permClaims,
                                expires: DateTime.Now.AddSeconds(5),
                                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string refeshToken)
        {
            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "http://www.noface.somee.com",
                    ValidAudience = "http://www.noface.somee.com",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("KeyBaoMatSieuCapVoDich"))
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(refeshToken, tokenValidationParameters, out SecurityToken securityToken);

                return principal;
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
        public IHttpActionResult RefeshToken(string refreshToken)
        {
            var principal = GetPrincipalFromExpiredToken(refreshToken);
            string idUser = principal.Claims.Where(p => p.Type == "idUser").FirstOrDefault()?.Value;

            var saveRefreshToken = GetRefreshToken(idUser);

            if (saveRefreshToken != refreshToken)
            {
                throw new SecurityTokenException("Hông phải user mà đòi lấy token");
            }

            DateTime exp = DateTime.Parse(principal.Claims.Where(p => p.Type == "Exp").FirstOrDefault()?.Value);

            if (DateTime.Now > exp)
            {
                return Ok(new Token("hết hạn", ""));
            }

            var newJwtToken = GenerateToken(idUser);
            var newRefreshToken = GenerateRefeshToken(idUser);
            SaveRefeshToken(idUser, newRefreshToken);
            return Ok(new Token(newJwtToken, newRefreshToken));
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
