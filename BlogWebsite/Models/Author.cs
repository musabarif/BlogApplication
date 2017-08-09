using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogWebsite.Models
{
    public class Author
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string Expertise { get; set; }
        public string About { get; set; }
      
    }
}