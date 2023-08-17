namespace ProductReviewAPI.Models
{
    public class UserTokens
    {
        public string Token
        {
            get;
            set;
        }
        public string Username
        {
            get;
            set;
        }
        public Role Role
        {
            get;
            set;
        }
        public TimeSpan Validaty
        {
            get;
            set;
        }
        public int UserId
        {
            get;
            set;
        }
        public Guid GuidId
        {
            get;
            set;
        }
        public DateTime ExpiredTime
        {
            get;
            set;
        }
    }
}
