using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogWebsite.Models
{
    public class ImageModel
    {
        public int ID { get; set; }
        public byte[] Data { get; set; }
        public string MimeType { get; set; }
        public string Username { get; set; }
    }
}