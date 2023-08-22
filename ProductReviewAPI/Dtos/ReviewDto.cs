namespace ProductReviewAPI.Dtos
{
    public class ReviewDto
    {
        public int ReviewId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public int UserId { get; set; }
    }
    public class ReviewCreateDTO
    {
        public string Comment { get; set; }
        public int Rating { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
    }

    public class ReviewUpdateDTO
    {
        public int ReviewId { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
    }
}
