using Blog.Models.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Blog.Models.ViewModels
{
    public class CommentHomeViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Body { get; set; }
        public DateTimeOffset DateUpdated { get; set; }
        public string ModifyingReason { get; set; }
        public virtual ApplicationUser User { get; set; }
        public string UserId { get; set; }
        public virtual BlogPost BlogPost { get; set; }
        public int BlogPostId { get; set; }
    }
}