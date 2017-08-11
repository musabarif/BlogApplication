using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogWebsite.Models
{
    public class Like
    {
        public int ID { get; set; }
        public int Post_ID { get; set; }
        public string Username { get; set; }
        public int Vote { get; set; }
        public int Flag { get; set; }
    }
}