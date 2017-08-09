using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogWebsite.Models
{
    public class ProfileView
    {
        public RegisterModel Register { get; set; }
        public Author Author { get; set; }
    }
}