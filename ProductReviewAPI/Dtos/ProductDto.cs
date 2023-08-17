using ProductReviewAPI.Models;

namespace ProductReviewAPI.Dtos
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public CategoryDto Category { get; set; }
        public ICollection<ProductImageDto> Images { get; set; }
        public ICollection<ReviewDto> Reviews { get; set; }
        public ICollection<SellerProductDTO> SellerProducts { get; set; }
    }

    public class ProductCreateDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
    }

    public class ProductUpdateDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
    }

    public class ProductDtoWithAvgRating
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public CategoryDto Category { get; set; }
        public ICollection<ProductImageDto> Images { get; set; }
        public ICollection<ReviewDto> Reviews { get; set; }
        public ICollection<SellerProductDTO> SellerProducts { get; set; }
        public double AverageRating { get; set; }
    }
}
