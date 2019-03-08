using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog.Models.Classes
{
    public class BlogPost
    {
        public int Int { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public bool Published { get; set; }
        public HttpPostedFileBase Image { get; set; }
        public int AuthorId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }

        public BlogPost()
        {
            DateCreated = DateTime.Now;
        }
    }
}