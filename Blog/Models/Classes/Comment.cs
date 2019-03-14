using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog.Models.Classes
{
    public class Comment
    {
        public int Id { get; set; }
        public string Body { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset? DateUpdated { get; set; }
        public string ModifyingReason { get; set; }

        public Comment()
        {
            DateCreated = DateTimeOffset.Now;
        }
    }
}