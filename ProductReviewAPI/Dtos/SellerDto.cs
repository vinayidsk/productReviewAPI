using ProductReviewAPI.Models;

namespace ProductReviewAPI.Dtos
{
    public class SellerDto
    {
        public int SellerId { get; set; }
        public string Name { get; set; }
        public ICollection<SellerProductDTO> SellerProducts { get; set; }
    }

    public class SellerProductDTO
    {
        public int SellerProductId { get; set; }
        public int SellerId { get; set; }
        public decimal Price { get; set; }
    }
}
