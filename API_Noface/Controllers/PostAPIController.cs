using API_Noface.Models;
using System;
using System.Collections;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Http;

namespace API_Noface.Controllers
{
    public class PostAPIController : ApiController
    {

        private readonly ArrayList loc = new ArrayList
                {
                    "gà",
                    "non", "Tú"
                };

        private readonly NofaceDbContext db = new NofaceDbContext();

        //get all bài viết theo 1 chủ đề
        [Authorize]
        [HttpGet]
        [Route("get-all-post-by-id/{id}")]
        public IHttpActionResult GetAllPostById(int id)//para là id topic
        {
            var posts = db.Post.Where(p => p.IDTopic == id)
                                            .OrderByDescending(p => p.Time)
                                            .Include(p => p.Likes)
                                            .Include(p => p.Comment)
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
                                            .OrderByDescending(p => p.Time)
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

                //lọc
                string tt = post.Title;
                string ct = post.Content;

                foreach (string item in loc)
                {
                    bool b1 = item.Contains(tt);
                    bool b2 = item.Contains(ct);
                    if (b1 == true || b2 == true)
                    {
                        User userDb = db.User.FirstOrDefault(u => u.IDUser.Equals(post.IDUser) == true);
                        userDb.Warning++;
                        db.User.AddOrUpdate(userDb);
                        db.SaveChanges();
                        return Ok(new Message(1, "Bài viết không phù hợp tiêu chuẩn, hãy chú ý ngôn từ nhé!"));
                    }
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
        public IHttpActionResult LikePost(int idpost, string iduser)
        {
            try
            {
                Likes likedb = db.Likes.FirstOrDefault(l => l.IDPost == idpost && l.IDUser.Equals(iduser) == true);

                if (likedb == null)
                {
                    Likes like = new Likes
                    {
                        IDPost = idpost,
                        IDUser = iduser
                    };

                    Post postdb = db.Post.FirstOrDefault(p => p.IDPost == idpost);
                    //tạo notification
                    Notification notification = new Notification
                    {
                        ID_Notification = 0,
                        ID_User = postdb.IDUser,
                        Data_Notification = "Bạn vừa có thêm 1 lượt thích",
                        IDPost = idpost,
                        ID_User_Seen_noti = iduser,
                        Status_Notification = 0
                    };

                    db.Notification.Add(notification);
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
        public IHttpActionResult GetAllLike(int id)//para là id post
        {
            var likes = db.Likes.Where(l => l.IDPost == id).ToList();
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

                //kt post đã xóa chưa
                Post post = db.Post.FirstOrDefault(p => p.IDPost == cmt.IDPost);
                if(post == null)
                {
                    return Ok(new Message(1, "Không thể bình luận do bài viết đã bị xóa!"));
                }

                //kt user có bị ban ko?
                User user = db.User.FirstOrDefault(u => u.IDUser.Equals(cmt.IDUser) == true);

                if(user.Activated == 0)
                {
                    return Ok(new Message(1, "Bạn không thể bình luận do đang thuộc danh sách hạn chế!"));
                }

                //lọc
                string ct = cmt.Content;

                foreach (string item in loc)
                {
                    bool b = item.Contains(ct);
                    if (b == true)
                    {
                        User userDb = db.User.FirstOrDefault(u => u.IDUser.Equals(cmt.IDUser) == true);
                        userDb.Warning++;
                        db.User.AddOrUpdate(userDb);
                        db.SaveChanges();
                        return Ok(new Message(1, "Bình luận không hợp lệ, hãy chú ý ngôn từ nhé!"));
                    }
                }

                //tạo notification
                Post postdb = db.Post.FirstOrDefault(p => p.IDPost == cmt.IDPost);
                Notification notification = new Notification
                {
                    ID_Notification = 0,
                    ID_User = postdb.IDUser,
                    Data_Notification = "Bạn vừa nhận được 1 bình luận",
                    IDPost = cmt.IDPost,
                    ID_User_Seen_noti = cmt.IDUser,
                    Status_Notification = 0
                };

                db.Notification.Add(notification);

                db.Comment.AddOrUpdate(cmt);
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Ok(new Message(0, "Có lỗi xảy ra rồi đại vương, hãy thử lại!"));
            }
            return Ok(new Message(1, "Hảo bình luận!"));
        }

        //20 bài viết trending
        [Authorize]
        [HttpGet]
        [Route("post-trending")]
        public IHttpActionResult PostTrending()
        {
            var posts = db.Post.OrderByDescending(p => p.Likes.Count + p.Comment.Count)
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
            var cmt = db.Comment.Where(c => c.IDPost == id)
                                .Include(c => c.LikeComment)
                                .OrderBy(c => c.Time)
                                .ToList();
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
                    var likecmt = db.LikeComment.Where(l => l.IDCmt == cmt.IDCmt).ToList();
                    foreach (LikeComment like in likecmt)
                    {
                        db.LikeComment.Remove(like);
                    }
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

        [Authorize]
        [HttpGet]
        [Route("new-post")]
        public IHttpActionResult NewPost()
        {
            var posts = db.Post.OrderByDescending(p => p.Time)
                               .Include(p => p.Likes)
                               .Include(p => p.Comment)
                               .ToList();

            if (posts == null)
            {
                return Ok(new Message(0, "Có lỗi xảy ra rồi đại vương, hãy thử lại!"));
            }
            return Ok(posts);
        }

        //like or unlike 1 comment
        [Authorize]
        [HttpPost]
        [Route("like-cmt/{idcmt}/{iduser}")]
        public IHttpActionResult LikeCmt(int idcmt, string iduser)
        {
            try
            {
                LikeComment likeCmtDb = db.LikeComment.FirstOrDefault(l => l.IDCmt == idcmt && l.IDUser.Equals(iduser) == true);

                if (likeCmtDb == null)
                {
                    LikeComment likeCmt = new LikeComment
                    {
                        IDCmt = idcmt,
                        IDUser = iduser
                    };
                    db.LikeComment.Add(likeCmt);
                }
                else
                {
                    db.LikeComment.Remove(likeCmtDb);
                }
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Ok(new Message(0, "Có lỗi xảy ra rồi đại vương, hãy thử lại!"));
            }
            return Ok(new Message(1, "cảm ơn bạn đã like <3"));
        }

        [Authorize]
        [HttpDelete]
        [Route("delete-cmt/{idcmt}")]
        public IHttpActionResult DeleteCmt(int idcmt)//para là id cmt
        {
            try
            {
                var cmt = db.Comment.Where(p => p.IDCmt == idcmt).FirstOrDefault();

                if (cmt == null)
                {
                    return Ok(new Message(0, "Không có cmt mà đòi xóa?"));
                }

                var likeCmt = db.LikeComment.Where(l => l.IDCmt == idcmt).ToList();

                foreach (LikeComment like in likeCmt)
                {
                    db.LikeComment.Remove(like);
                }

                db.Comment.Remove(cmt);
                db.SaveChanges();
                return Ok(new Message(1, "Xóa bình luận thành công!"));
            }
            catch (Exception)
            {
                return Ok(new Message(0, "Có lỗi xảy ra rồi đại vương, hãy thử lại!"));
            }
        }

        [Authorize]
        [HttpPost]
        [Route("view-post/{idpost}")]
        public IHttpActionResult ViewPost(int idpost)//para là id post
        {
            try
            {
                var post = db.Post.FirstOrDefault(p => p.IDPost == idpost);

                if (post == null)
                {
                    return Ok(new Message(0, "Bài viết này đã bị xóa"));
                }

                post.Views++;

                db.Post.AddOrUpdate(post);
                db.SaveChanges();
                return Ok(new Message(1, "ok thành công, đừng show lên"));
            }
            catch (Exception)
            {
                return Ok(new Message(0, "Có lỗi xảy ra rồi đại vương, hãy thử lại!"));
            }
        }
    }
}
