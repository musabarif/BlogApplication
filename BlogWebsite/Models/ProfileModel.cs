using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace BlogWebsite.Models
{
    public class ProfileModel
    {
        public MembershipUser User { get; set; }
    }
}