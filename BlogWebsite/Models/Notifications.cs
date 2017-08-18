using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogWebsite.Models
{
    public class Notifications
    {
        public int ID { get; set; }
        public string UserName{ get; set; }
        public string Name { get; set; }
        public string Action { get; set; }
        public int Post_ID { get; set; }
        public int Flag { get; set; }
    }
}