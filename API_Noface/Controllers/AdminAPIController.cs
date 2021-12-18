using API_Noface.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
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
        private readonly NofaceDbContext db = new NofaceDbContext();

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
            var LstUser = db.User.Include(u => u.Ban).ToList();
            return Ok(LstUser);
        }

        //block
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("block-user/{IDUser}")]
        public IHttpActionResult BlockUser(string IDUser)
        {
            User user = db.User.FirstOrDefault(u => u.IDUser.Equals(IDUser) == true);
            Ban userBan = db.Ban.FirstOrDefault(u => u.IDUser.Equals(IDUser) == true);

            if (user != null && userBan == null)
            {

                user.Warning = 0;
                user.Activated = 0;

                Ban ban = new Ban
                {
                    IDBan = 0,
                    IDUser = IDUser,
                    TimeBan = DateTime.Now.AddDays(10)
                };

                //DateTime time = DateTime.Now.AddDays(10);
                //tạo notification
                Notification notification = new Notification
                {
                    ID_Notification = 0,
                    ID_User = IDUser,
                    IDPost = null,
                    Data_Notification = "Tài khoản của bạn đã bị hạn chế!",
                    ID_User_Seen_noti = "Admin",
                    Status_Notification = 0
                };

                db.Notification.Add(notification);
                db.SaveChanges();
                db.Ban.Add(ban);
                db.SaveChanges();

                return Ok(new Message(1, "Đã khóa tài khoản thành công!"));
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

            if (user != null && userBan != null)
            {
                db.Ban.Remove(userBan);
                user.Activated = 1;

                //tạo notification
                Notification notification = new Notification
                {
                    ID_Notification = 0,
                    ID_User = IDUser,
                    IDPost = null,
                    Data_Notification = "Tài khoản của bạn đã được mở khóa!",
                    ID_User_Seen_noti = "Admin",
                    Status_Notification = 0
                };

                db.Notification.Add(notification);
                db.SaveChanges();
                return Ok(new Message(1, "Đã mở khóa tài khoản thành công!"));
            }
            return Ok(new Message(0, "Có lỗi xảy ra rồi đại vương, hãy thử lại!"));
        }

        //notification
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("get-all-cmt-user/{IDUser}")]
        public IHttpActionResult GetAllCmtUser(string IDUser)
        {
            var cmts = db.Comment.Where(c => c.IDUser.Equals(IDUser) == true)
                                 .Include(c => c.Post)
                                 .ToList();
            return Ok(cmts);
        }

        //unblock
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("add-notification")]
        public IHttpActionResult AddNotification(Notification notification)
        {
            try
            {
                Notification notificationDb = new Notification
                {
                    ID_Notification = 0,
                    ID_User = notification.ID_User,
                    Data_Notification = notification.Data_Notification,
                    IDPost = notification.IDPost,
                    ID_User_Seen_noti = "Admin",
                    Status_Notification = 0
                };
                db.Notification.Add(notificationDb);
                db.SaveChanges();
                return Ok(new Message(1, "Đã thêm thông báo thành công!"));
            }
            catch
            {
                return Ok(new Message(0, "Có lỗi xảy ra rồi đại vương, hãy thử lại!"));
            }
        }

        //get ban
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("get-ban-user")]
        public IHttpActionResult GetBanUser()
        {
            var LstUser = db.Ban.ToList();
            return Ok(LstUser);
        }

        //thống kê 2
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("get-statistics")]
        public IHttpActionResult GetStatistics()
        {
            var posts = db.Post.ToList();
            var users = db.User.ToList();
            int i = 1;
            int sumPost = 0;
            int sumUser = 0;
            int[] LstPost = new int[12];
            int[] LstUser = new int[12];

            while (i <= 12)
            {
                foreach(var item in posts)
                {
                    DateTime dt = (DateTime)item.Time;
                    if(dt.Month == i)
                    {
                        sumPost++;
                    }
                }
                LstPost[i-1] += sumPost;
                sumPost = 0;
                i++;
            }

            //User
            i = 1;
            while (i <= 12)
            {
                foreach (var item in users)
                {
                    DateTime dt = (DateTime)item.TimeRegister;
                    if (dt.Month == i)
                    {
                        sumUser++;
                    }
                }
                LstUser[i - 1] += sumUser;
                sumUser = 0;
                i++;
            }


            return Ok(new Statistics(LstPost, LstUser));
        }

        //thống kê 1
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("get-SumStatistics")]
        public IHttpActionResult GetSumStatistics()
        {
            var posts = db.Post.ToList();
            var users = db.User.ToList();
            int PostsMonth = 0;
            int UserMonth = 0;

            foreach (var item in posts)
            {
                int now = DateTime.Now.Month;
                DateTime dt = (DateTime)item.Time;
                if (dt.Month == now)
                {
                    PostsMonth++;
                }
            }

            foreach (var item in users)
            {
                int now = DateTime.Now.Month;
                DateTime dt = (DateTime)item.TimeRegister;
                if (dt.Month == now)
                {
                    UserMonth++;
                }
            }

            int SumPosts = db.Post.Count();
            int SumUsers = db.User.Count();

            return Ok(new SumStatistics(SumPosts, SumUsers, UserMonth, PostsMonth));
        }
    }
}
