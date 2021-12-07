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
    [Route("/api/")]
    public class UserAPIController : ApiController
    {
        private readonly NofaceDbContext db = new NofaceDbContext();

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

        //post danh hiệu
        [Authorize]
        [Route("Check-Achievements/{iduser}")]
        [HttpPost]
        public IHttpActionResult AddAchievement(string iduser)
        {

            User user = db.User.Where(u => u.IDUser.Equals(iduser) == true)
                            .Include(u => u.Likes)
                            .Include(u => u.Comment).FirstOrDefault();

            if (user.Comment.Count > 9)
            {
                //cmt 
                var ach = db.Achievements.Where(a => a.IDUser.Equals(iduser) == true && a.IDMedal == 4).FirstOrDefault();
                if (ach == null)
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

            if (user.Likes.Count > 9)
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

            if (user.Post.Count > 9)
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

        //danh hiệu
        [Authorize]
        [Route("DanhHieu/{iduser}")]
        [HttpGet]
        public IHttpActionResult Achievement(string iduser)
        {
            var achie = db.Achievements.Where(a => a.IDUser.Equals(iduser) == true)
                          .Include(a => a.Medals)
                          .ToList();

            return Ok(achie);
        }

        //danh hiệu
        [Authorize]
        [Route("Follower")]
        [HttpPost]
        public IHttpActionResult CreateFriend(Friends friend)//para 0, iduser, idfriend, 1
        {
            db.Friends.Add(friend);
            db.SaveChanges();

            return Ok(new Message(1, "follow thành công"));
        }

        [Authorize]
        [Route("Accept")]
        [HttpPost]
        public IHttpActionResult Accept(Friends friend)//para 0, iduser, idfriend, 1
        {

            Friends friends = db.Friends.FirstOrDefault(f => f.IDUser.Equals(friend.IDFriends) == true && f.IDFriends.Equals(friend.IDUser) == true);

            friends.Status = 3;

            db.Friends.AddOrUpdate(friends);
            db.SaveChanges();

            return Ok(new Message(1, "kết bạn thành công"));
        }

        [Authorize]
        [Route("DeleteFriend")]
        [HttpPost]
        public IHttpActionResult DeleteFriend(Friends friend)//para 0, iduser, idfriend, 1
        {

            Friends friendsAdd = db.Friends.FirstOrDefault(f => (f.IDUser.Equals(friend.IDUser) == true && f.IDFriends.Equals(friend.IDFriends) == true && f.Status == 3) || f.IDUser.Equals(friend.IDFriends) == true && f.IDFriends.Equals(friend.IDUser) == true && f.Status == 3 || f.IDUser.Equals(friend.IDUser) && f.IDFriends.Equals(friend.IDFriends) == true && f.Status == 1);

            db.Friends.Remove(friendsAdd);
            db.SaveChanges();

            return Ok(new Message(1, "xóa bạn thành công"));
        }

        [Authorize]
        [Route("List-Follower")]
        [HttpPost]
        public IHttpActionResult ListFollower(string idUser)//para là my iduser
        {
            var friends = db.Friends.Where(f => f.IDFriends.Equals(idUser) && f.Status == 1).ToList();
            return Ok(friends);
        }

        [Authorize]
        [Route("List-Friend")]
        [HttpPost]
        public IHttpActionResult ListFriend(string idUser)//para là my iduser
        {
            var friends = db.Friends.Where(f => f.IDFriends.Equals(idUser) && f.Status == 3 || f.IDUser.Equals(idUser) && f.Status == 3).ToList();
            return Ok(friends);
        }

        [Authorize]
        [Route("CheckFriends/{iduser}/{idfriend}")]
        [HttpGet]
        public IHttpActionResult CheckFriend(string iduser, string idfriend)
        {
            Friends friendsAdd = db.Friends.FirstOrDefault(f => (f.IDUser.Equals(iduser) == true && f.IDFriends.Equals(idfriend) == true && f.Status == 3) || f.IDUser.Equals(idfriend) == true && f.IDFriends.Equals(iduser) == true && f.Status == 3);

            Friends friendsFollower = db.Friends.FirstOrDefault(f => f.IDUser.Equals(iduser) == true && f.IDFriends.Equals(idfriend) == true && f.Status == 1);

            Friends friendsFollowee = db.Friends.FirstOrDefault(f => f.IDUser.Equals(idfriend) == true && f.IDFriends.Equals(iduser) == true && f.Status == 1);

            if (friendsAdd != null)
            {
                return Ok(new Message(3, "Đã kết bạn"));
            }

            if (friendsFollower != null)
            {
                return Ok(new Message(1, "Theo dõi"));
            }

            if (friendsFollowee != null)
            {
                return Ok(new Message(2, "Được theo dõi"));
            }

            return Ok(new Message(0, "Người lạ"));
        }

        //get danh sách thông báo
        [Authorize]
        [Route("notification/{idUser}")]
        [HttpPost]
        public IHttpActionResult Notification(string idUser)
        {
            var noti = db.Notification.Where(n => n.ID_User.Equals(idUser) == true).ToList();

            return Ok(noti);
        }

        [Authorize]
        [HttpPost]
        [Route("view-noti/{idNoti}")]
        public IHttpActionResult ViewNoti(int idNoti)//para là id noti
        {
            try
            {
                var noti = db.Notification.FirstOrDefault(n => n.ID_Notification == idNoti);

                if (noti == null)
                {
                    return Ok(new Message(0, "Lỗi"));
                }

                if(noti.Status_Notification == 0)
                {
                    noti.Status_Notification = 1;
                }

                db.Notification.AddOrUpdate(noti);
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

