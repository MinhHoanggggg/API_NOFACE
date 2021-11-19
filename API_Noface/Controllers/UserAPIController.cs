using API_Noface.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API_Noface.Controllers
{
    [Route("/api/")]
    public class UserAPIController : ApiController
    {
        private NofaceDbContext db = new NofaceDbContext();

        //get 1 user theo id
        [Authorize]
        [Route("get-user-by-id/{id}")]
        [HttpGet]
        public IHttpActionResult GetUserById(string id)
        {
            var user = db.User.Where(u => u.IDUser.Equals(id))
                                               .Include(u => u.Post)
                                               .FirstOrDefault();

            if (user == null)
            {
                return Ok(new Message(0, "Có lỗi xảy ra rồi đại vương!"));
            }
            return Ok(user);
        }

        //get post by id user
        [Authorize]
        [HttpGet]
        [Route("get-all-post-by-user/{id}")]
        public IHttpActionResult GetAllPostById(string id)//para là id user
        {
            var posts = db.Post.Where(p => p.IDUser.Equals(id))
                                            .Include(p => p.Likes)
                                            .Include(p => p.Comment)
                                            .ToList();

            if (posts == null)
            {
                return Ok(new Message(0, "Có lỗi xảy ra rồi đại vương, hãy thử lại!"));
            }
            return Ok(posts);
        }

        //thêm 1 user mới
        [Route("create-user")]
        [HttpPost]
        public IHttpActionResult CreateUser(User user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new Message(0, "Đăng kí không thành công"));
                }

                db.User.Add(user);
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Ok(new Message(0, "Đăng kí không thành công"));
            }
            return Ok(new Message(1, "Chúc mừng bạn đã đăng kí thành công"));
        }

        //get achievements by id user
        [Authorize]
        [HttpGet]
        [Route("achievements/{id}")]
        public IHttpActionResult Achievements(string id)//para là id user
        {
            var achieve = db.Achievements.Where(p => p.IDUser.Equals(id)).ToList();
            return Ok(achieve);
        }


        //xóa 1 user
        [Route("delete-user/{iduser}")]
        [HttpPost]
        public IHttpActionResult DeleteUser(string iduser)
        {
            var user = db.User.Where(u => u.IDUser.Equals(iduser))
                                               .Include(u => u.Post)
                                               .FirstOrDefault();
            try
            {
                db.User.Remove(user);
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Ok(new Message(0, "xóa không thành công"));
            }
            return Ok(new Message(1, "Chúc mừng bạn đã xóa thành công"));
        }

        //danh hiệu
        [Authorize]
        [Route("Achievements/{iduser}")]
        [HttpGet]
        public IHttpActionResult Achievement(string iduser)
        {
            var achie = db.Achievements.Where(a => a.IDUser.Equals(iduser) == true).ToList();

            return Ok(achie);
        }

        //post danh hiệu
        [Authorize]
        [Route("Add-Achievements/{iduser}")]
        [HttpPost]
        public IHttpActionResult AddAchievement(string iduser)
        {

            User user = db.User.FirstOrDefault(u => u.IDUser.Equals(iduser) == true);

            if(user.Comment.Count > 9)
            {
                //cmt 
                var ach = db.Achievements.Where(a => a.IDUser.Equals(iduser) == true && a.IDMedal == 4).FirstOrDefault();
                if(ach == null)
                {
                    Achievements achievements = new Achievements
                    {
                        IDMedal = 4,
                        IDUser = iduser
                    };
                    db.Achievements.Add(achievements);
                    db.SaveChanges();
                    return Ok(new Message(4, "Chúc mừng bạn đã nhận được 1 danh hiệu mới!"));
                }
            }

            if(user.Likes.Count > 9)
            {
                //like
                var ach = db.Achievements.Where(a => a.IDUser.Equals(iduser) == true && a.IDMedal == 3).FirstOrDefault();
                if (ach == null)
                {
                    Achievements achievements = new Achievements
                    {
                        IDMedal = 3,
                        IDUser = iduser
                    };
                    db.Achievements.Add(achievements);
                    db.SaveChanges();
                    return Ok(new Message(3, "Chúc mừng bạn đã nhận được 1 danh hiệu mới!"));
                }
            }

            if(user.Post.Count > 9)
            {
                //post
                var ach = db.Achievements.Where(a => a.IDUser.Equals(iduser) == true && a.IDMedal == 2).FirstOrDefault();
                if (ach == null)
                {
                    Achievements achievements = new Achievements
                    {
                        IDMedal = 2,
                        IDUser = iduser
                    };
                    db.Achievements.Add(achievements);
                    db.SaveChanges();
                    return Ok(new Message(2, "Chúc mừng bạn đã nhận được 1 danh hiệu mới!"));
                }
            }

            if (user.Post.Count == 0)
            {
                //new user
                var ach = db.Achievements.Where(a => a.IDUser.Equals(iduser) == true && a.IDMedal == 1).FirstOrDefault();
                if (ach == null)
                {
                    Achievements achievements = new Achievements
                    {
                        IDMedal = 1,
                        IDUser = iduser
                    };
                    db.Achievements.Add(achievements);
                    db.SaveChanges();
                    return Ok(new Message(1, "Chúc mừng bạn đã nhận được 1 danh hiệu mới!"));
                }
            }
            return Ok(new Message(0, ""));
        }
    }
}
