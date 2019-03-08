using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog.Models.Classes
{
    public class Author
    {
        public int Id { get; set; }
        public List<BlogPost> Posts { get; set; } = new List<BlogPost>();
    }
}