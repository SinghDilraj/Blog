﻿using Blog.Models.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blog.Models.ViewModels
{
    public class CreateHomeViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        [AllowHtml]
        public string Body { get; set; }

        [Required]
        public bool Published { get; set; }

        [Required]
        public HttpPostedFileBase Image { get; set; }

        public List<Comment> Comments { get; set; } = new List<Comment>();

        public string Slug { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }

        public string ImageUrl { get; set; }
    }
}