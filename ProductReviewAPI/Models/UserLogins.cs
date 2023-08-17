using System.ComponentModel.DataAnnotations;
namespace ProductReviewAPI.Models
{
    public class UserLogins
    {
        [Required]
        public string Username
        {
            get;
            set;
        }
        [Required]
        public string Password
        {
            get;
            set;
        }
        public UserLogins() { }
    }
}
