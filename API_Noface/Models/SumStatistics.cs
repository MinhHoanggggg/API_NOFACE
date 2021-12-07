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

        public int SumPosts { get; }
        public int SumUsers { get; }
        public int UserMonth { get; }
        public int PostsMonth { get; }
    }
}