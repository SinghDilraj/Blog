using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Blog.Models.ViewModels
{
    public class CreateHomeViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Body { get; set; }

        [Required]
        public bool Published { get; set; }

        [Required]
        public HttpPostedFileBase Image { get; set; }

        public DateTimeOffset DateCreated { get; set;}

        public DateTimeOffset DateUpdated {get; set; }

        public string ImageUrl { get; set; }
    }
}