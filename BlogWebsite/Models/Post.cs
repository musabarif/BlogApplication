using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogWebsite.Models
{
    public class Post
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string Date { get; set; }
        public List<Tag> Tags { get; set; }
        public string Author { get; set; }
        public int Views { get; set; }

        public Post()
        {
            this.Views = 0;
        }
    }
}