using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlogWebsite.Models
{
    public class Post
    {
        public int ID { get; set; }
        public string Title { get; set; }

        [AllowHtml]
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }

        public string Date { get; set; }
        public List<Tag> Tags { get; set; }
        public string Author { get; set; }
        public int Views { get; set; }
        public int Votes { get; set; }

        public Post()
        {
            this.Views = 0;
        }
    }
}