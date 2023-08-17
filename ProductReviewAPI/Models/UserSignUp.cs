using System.ComponentModel.DataAnnotations;
namespace ProductReviewAPI.Models
{
    public class UserSignUp
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

        public string FirstName
        {
            get;
            set;
        }
        public string LastName
        {
            get;
            set;
        }
        public UserSignUp() { }
    }
}
