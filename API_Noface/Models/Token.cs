using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_Noface.Models
{
    public partial class Token
    {
        public string data { get; set; }

        public string RefreshToken { get; set; }

        public Token(string data, string refreshToken)
        {
            this.data = data;
            RefreshToken = refreshToken;
        }
    }
}