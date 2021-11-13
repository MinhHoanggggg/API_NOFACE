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
        private NofaceDbContext db = new NofaceDbContext();

        [Route("get-token/{idUser}")]
        [HttpPost]
        public IHttpActionResult GetToken(string idUser)
        {
            var user = db.User.Where(u => u.IDUser.Equals(idUser) == true).FirstOrDefault();

            if (user != null)
            {
                string key = "KeyBaoMatSieuCapVoDich"; //Secret key which will be used later during validation    
                var issuer = "http://www.noface.somee.com";  //normally this will be your site URL    

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var permClaims = new List<Claim>();
                permClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                permClaims.Add(new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString()));
                permClaims.Add(new Claim("idUser", user.IDUser));

                var token = new JwtSecurityToken(issuer,
                                issuer,
                                permClaims,
                                expires: DateTime.Now.AddDays(1),
                                signingCredentials: credentials);

                var jwt_token = new JwtSecurityTokenHandler().WriteToken(token);
                return Ok(new Token(new JwtSecurityTokenHandler().WriteToken(token)));
            }
            else
            {
                return BadRequest("Hông phải user mà đòi lấy api");
            }
        }
    }
}
