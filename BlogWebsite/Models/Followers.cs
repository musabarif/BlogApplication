using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogWebsite.Models
{
    public class Followers
    {
        public int ID { get; set; }
        public int Leader { get; set; }
        public int Follower { get; set; }
    }
}