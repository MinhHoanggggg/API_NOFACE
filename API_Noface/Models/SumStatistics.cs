using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_Noface.Models
{
    public class SumStatistics
    {
        public SumStatistics(int sumPosts, int sumUsers, int userMonth, int postsMonth)
        {
            SumPosts = sumPosts;
            SumUsers = sumUsers;
            UserMonth = userMonth;
            PostsMonth = postsMonth;
        }

        public int SumPosts { get; set; }
        public int SumUsers { get; set; }
        public int UserMonth { get; set; }
        public int PostsMonth { get; set; }
    }
}