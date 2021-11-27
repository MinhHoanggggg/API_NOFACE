using API_Noface.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Web.Http;

namespace API_Noface.Controllers
{
    public class AdminAPIController : ApiController
    {
        private NofaceDbContext db = new NofaceDbContext();

        public string GenerateToken(string UserAdmin, string PassAdmin)
        {
            string key = "KeyBaoMatSieuCapVoDich";
            var issuer = "http://www.noface.somee.com";
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var permClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString()),
                new Claim("UserAdmin", UserAdmin),
                new Claim("PassAdmin", PassAdmin),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var token = new JwtSecurityToken(issuer,
                                issuer,
                                permClaims,
                                expires: DateTime.Now.AddHours(3),
                                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [Route("get-token-admin")]
        [HttpPost]
        public IHttpActionResult GetTokenAdmin(Admin admin)
        {
            var Admin = db.Admin.FirstOrDefault(a => a.UserAdmin.Equals(admin.UserAdmin) 
                                                  && a.PassAdmin.Equals(admin.PassAdmin));

            if (Admin != null)
            {
                var tokenAd = GenerateToken(admin.UserAdmin, admin.PassAdmin);
                return Ok(new Message(1, tokenAd));
            }

            return Ok(new Message(0, "Không phải admin mà đòi token"));
        }

        //get all user
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("get-all-user")]
        public IHttpActionResult GetAllUser()
        {
            var LstUser = db.User.ToList();
            return Ok(LstUser);
        }

        //block
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("block-user/{IDUser}")]
        public IHttpActionResult BlockUser(string IDUser)
        {
            User user = db.User.FirstOrDefault(u => u.IDUser.Equals(IDUser) == true);

            if(user != null)
            {
                Ban ban = new Ban
                {
                    IDBan = 0,
                    IDUser = IDUser,
                    TimeBan = DateTime.Now.AddDays(10)
                };

                db.Ban.Add(ban);
                user.Warning = 0;
                user.Activated = 0;
                db.SaveChanges();
                return Ok(new Message(1, "Đã ban tài khoản thành công!"));
            }
            return Ok(new Message(0, "Có lỗi xảy ra rồi đại vương, hãy thử lại!"));
        }

        //unblock
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("unblock-user/{IDUser}")]
        public IHttpActionResult UnblockUser(string IDUser)
        {
            User user = db.User.FirstOrDefault(u => u.IDUser.Equals(IDUser) == true);
            var userBan = db.Ban.FirstOrDefault(u => u.IDUser.Equals(IDUser) == true);

            if (user != null && userBan != null && userBan.TimeBan < DateTime.Now)
            {
                db.Ban.Add(userBan);
                user.Activated = 1;
                db.SaveChanges();
                return Ok(new Message(1, "Đã mở khóa tài khoản thành công!"));
            }
            return Ok(new Message(0, "Có lỗi xảy ra rồi đại vương, hãy thử lại!"));
        }

        //unblock
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("get-all-cmt-user/{IDUser}")]
        public IHttpActionResult GetAllCmtUser(string IDUser)
        {
            var cmts = db.Comment.Where(c => c.IDUser.Equals(IDUser) == true)
                                 .Include(c => c.IDPost)
                                 .ToList();
            return Ok(cmts);
        }

    }
}
