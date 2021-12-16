using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_Noface.Models
{
    public class Statistics
    {
        public Statistics(int[] posts, int[] users)
        {
            Posts = posts;
            Users = users;
        }

        public int[] Posts { get; set; }
        public int[] Users { get; set; }
    }
}