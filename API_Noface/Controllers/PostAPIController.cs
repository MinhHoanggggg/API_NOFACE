using API_Noface.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API_Noface.Controllers
{
    public class PostAPIController : ApiController
    {
        private NofaceDbContext db = new NofaceDbContext();

        //get all bài viết theo 1 chủ đề
        [Authorize]
        [HttpGet]
        [Route("get-all-post-by-id/{id}")]
        public IHttpActionResult GetAllPostById(int id)//para là id topic
        {
            var posts = db.Post.Where(p => p.IDTopic == id)
                                            .Include(p => p.Likes)
                                            .Include(p => p.Comment)
                                            .Include(p => p.Topic)
                                            .ToList();

            if (posts == null)
            {
                return Ok(new Message(0, "Có lỗi xảy ra rồi đại vương, hãy thử lại!"));
            }
            return Ok(posts);
        }

        //get 1 bài viết
        [Authorize]
        [HttpGet]
        [Route("get-post-by-id/{id}")]
        public IHttpActionResult GetPostById(int id)//para là id post
        {
            var posts = db.Post.Where(p => p.IDPost == id)
                                            .Include(p => p.Likes)
                                            .Include(p => p.Comment)
                                            .FirstOrDefault();

            if (posts == null)
            {
                return Ok(new Message(0, "Có lỗi xảy ra rồi đại vương, hãy thử lại!"));
            }
            return Ok(posts);
        }

        //thêm or sửa 1 bài viết theo chủ đề
        [Authorize]
        [HttpPost]
        [Route("post-post")]
        public IHttpActionResult PostPost(Post post)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new Message(0, "Có lỗi xảy ra rồi đại vương, hãy thử lại!"));
                }

                db.Post.AddOrUpdate(post);
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Ok(new Message(0, "Có lỗi xảy ra rồi đại vương, hãy thử lại!"));
            }
            return Ok(new Message(1, "Thành công rồi đại vương ơi!"));
        }

        //like or unlike 1 bài viết
        [Authorize]
        [HttpPost]
        [Route("like-post/{idpost}/{iduser}")]
        public IHttpActionResult LikePost(int idpost, String iduser)
        {
            try
            {
                Likes likedb = db.Likes.FirstOrDefault(l => l.IDPost == idpost && l.IDUser.Equals(iduser) == true);

                if (likedb == null)
                {
                    Likes like = new Likes();
                    like.IDPost = idpost;
                    like.IDUser = iduser;
                    db.Likes.Add(like);
                }
                else
                {
                    db.Likes.Remove(likedb);
                }
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Ok(new Message(0, "Có lỗi xảy ra rồi đại vương, hãy thử lại!"));
            }
            return Ok(new Message(1, "cảm ơn bạn đã like <3"));
        }

        //get like by id post
        [Authorize]
        [HttpGet]
        [Route("get-all-like-by-id/{id}")]
        public IHttpActionResult GetAllLikeCmt(int id)//para là id post
        {
            var likes = db.Likes.Where(l => l.IDPost == id).ToList();

            if (likes == null)
            {
                return Ok(new Message(0, "Có like nào đâu mà get"));
            }

            return Ok(likes);
        }


        //thêm or sửa 1 cmt
        [Authorize]
        [HttpPost]
        [Route("post-cmt")]
        public IHttpActionResult PostCmt(Comment cmt)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new Message(0, "Có lỗi xảy ra rồi đại vương, hãy thử lại!"));
                }

                db.Comment.AddOrUpdate(cmt);
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Ok(new Message(0, "Có lỗi xảy ra rồi đại vương, hãy thử lại!"));
            }
            return Ok(new Message(1, "Bình luận hay quá đại vương!"));
        }

        //20 bài viết trending
        [Authorize]
        [HttpGet]
        [Route("post-trending")]
        public IHttpActionResult PostTrending()
        {
            var posts = db.Post.OrderByDescending(p => p.Likes.Count)
                               .Include(p => p.Likes)
                               .Include(p => p.Comment)
                               .ToList()
                               .Take(20);

            if (posts == null)
            {
                return Ok(new Message(0, "Có lỗi xảy ra rồi đại vương, hãy thử lại!"));
            }
            return Ok(posts);
        }

        //get all cmt của post
        [Authorize]
        [HttpGet]
        [Route("get-all-cmt-by-id/{id}")]
        public IHttpActionResult GetAllCmt(int id)//para là id post
        {
            var cmt = db.Comment.Where(c => c.IDPost == id).ToList();

            if (cmt == null)
            {
                return Ok(new Message(0, "Có cmt nào đâu mà get"));
            }

            return Ok(cmt);
        }

        [Authorize]
        [HttpDelete]
        [Route("delete-post/{id}")]
        public IHttpActionResult DeletePost(int id)//para là id post
        {
            try
            {
                var posts = db.Post.Where(p => p.IDPost == id).FirstOrDefault();

                if (posts == null)
                {
                    return Ok(new Message(0, "Không có post mà đòi xóa?"));
                }

                var likes = db.Likes.Where(l => l.IDPost == id).ToList();

                foreach (Likes like in likes)
                {
                    db.Likes.Remove(like);
                }

                var cmts = db.Comment.Where(c => c.IDPost == id).ToList();

                foreach (Comment cmt in cmts)
                {
                    db.Comment.Remove(cmt);
                }

                db.Post.Remove(posts);
                db.SaveChanges();
                return Ok(new Message(1, "Xóa bài viết thành công!"));
            }
            catch (Exception)
            {
                return Ok(new Message(0, "Có lỗi xảy ra rồi đại vương, hãy thử lại!"));
            }
        }
    }
}
