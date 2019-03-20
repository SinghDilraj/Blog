using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog.Models.Classes
{
    public class BlogPost
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public bool Published { get; set; }
        public virtual ApplicationUser User { get; set; }
        public string UserId { get; set; }
        public string Image { get; set; }
        public string Slug { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public virtual List<Comment> Comments { get; set; }

        public BlogPost()
        {
            Comments = new List<Comment>();
            DateCreated = DateTime.Now;
        }
    }
}