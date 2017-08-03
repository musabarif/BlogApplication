using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogWebsite.Models
{
    public class IndexModel
    {
        public Post post { get; set; }
        public Tag tag { get; set; }
    }
}