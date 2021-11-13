using API_Noface.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API_Noface.Controllers
{
    public class TopicAPIController : ApiController
    {
        private NofaceDbContext db = new NofaceDbContext();

        [Authorize]
        [Route("get-all-topic")]
        [HttpGet]
        public IHttpActionResult GetAllTopic()
        {
            return Ok(db.Topic.ToList());
        }

        [Authorize]
        [Route("add-topic")]
        [HttpPost]
        public IHttpActionResult AddTopic(Topic topic)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new Message(0, "Có lỗi xảy ra rồi đại vương, thêm chủ đề không thành công!"));
                }

                //kiểm tra db
                if (db.Topic.FirstOrDefault(t => t.TopicName.Equals(topic.TopicName)) != null)
                {
                    return Ok(new Message(0, "Có lỗi xảy ra rồi đại vương, chủ đề đã tồn tại!"));
                }

                db.Topic.Add(topic);
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Ok(new Message(0, "Có lỗi xảy ra rồi đại vương, thêm chủ đề không thành công!"));
            }
            return Ok(new Message(1, "Thêm chủ đề thành công rồi đại vương ơi!"));
        }

        //only admin
        [Authorize]
        [Route("delete-topic/{id}")]
        [HttpDelete()]
        public IHttpActionResult DeleteTopic(int id)
        {
            try
            {
                var topic = db.Topic.FirstOrDefault(t => t.IDTopic == id);
                if (topic != null)
                {
                    //like, cmt, post
                    db.Topic.Remove(topic);
                    db.SaveChanges();
                    return Ok(1);
                }
                else
                {
                    return Ok(0);
                }
            }
            catch (Exception)
            {
                return Ok(0);
            }
        }

        //[Route("update-topic")]
        //[HttpPost]
        //public IHttpActionResult UpdateTopic(Topic topic)
        //{
        //    var topics = db.Topic.FirstOrDefault(t => t.IDTopic == id);

        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return Ok(new Message(0, "Có biến rồi đại vương ơi, đã xãy ra lỗi!"));
        //        }
        //        db.Topic.Add(topic);
        //        db.SaveChanges();
        //    }
        //    catch (Exception)
        //    {
        //        return Ok(new Message(0, "Có biến rồi đại vương ơi, đã xãy ra lỗi!"));
        //    }
        //    return Ok(new Message(1, "Thêm chủ đề thành công"));
        //}
    }
}
