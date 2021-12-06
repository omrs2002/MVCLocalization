using System.ComponentModel.DataAnnotations;

namespace MVCLocalization.Web.Models
{
    public class AuthorViewModel
    {
        [Display(Name = "FirstName")]
        [Required(ErrorMessage = "{0} is required")]
        public string FirstName { get; set; }

        [Display(Name = "LastName")]
        [Required(ErrorMessage = "{0} is required")]
        public string LastName { get; set; }

        [Display(Name = "BooksAuthored")]
        [Range(1, 99, ErrorMessage = "{0} must be a number between {1} and {2}")]
        public int BooksAuthored { get; set; }
    }
}
