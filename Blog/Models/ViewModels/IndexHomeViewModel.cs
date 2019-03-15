using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Blog.Models.ViewModels
{
    public class IndexHomeViewModel
    {
        [Required]
        public string Query { get; set; }
    }
}