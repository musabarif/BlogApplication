﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogWebsite.Models
{
    public class Comment
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Comments { get; set; }
        public int PostID { get; set; }
        public int ParentID { get; set; }
        public string CommentTime { get; set; }
    }
}