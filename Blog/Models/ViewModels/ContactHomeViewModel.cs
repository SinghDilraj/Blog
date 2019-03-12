using System.ComponentModel.DataAnnotations;

namespace Blog.Models.ViewModels
{
    public class ContactHomeViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Number { get; set; }

        [Required]
        public string Message { get; set; }
    }
}