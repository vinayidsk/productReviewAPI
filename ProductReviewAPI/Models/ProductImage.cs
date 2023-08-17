using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductReviewAPI.Models
{
    [Table("ProductImages")]
    public class ProductImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ImageId { get; set; }
        public string? Url { get; set; }
        public int ProductId { get; set; } // Foreign key referencing Product
        [ForeignKey("ProductId")]
        public Product? Product { get; set; } // Navigation property
    }
}
