using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductReviewAPI.Models
{
    [Table("Sellers")]
    public class Seller
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SellerId { get; set; }
        public string? Name { get; set; }
    }

    [Table("SellerProduct")]
    public class SellerProduct
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SellerProductId { get; set; }
        public int SellerId { get; set; }
        public int ProductId { get; set; }
        public decimal Price { get; set; }
    }
}
