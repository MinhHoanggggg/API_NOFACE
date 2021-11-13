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
    }
}
