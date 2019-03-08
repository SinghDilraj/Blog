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
        public string Subtitle { get; set; }
        public string ImageURL { get; set; }
        public int AuthorId { get; set; }
        public DateTime DateCreated { get; set; }

        public BlogPost()
        {
            DateCreated = DateTime.Now;
        }
    }
}