using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductReviewAPI.Dtos;
using ProductReviewAPI.Models;
using ProductReviewAPI.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Linq.Expressions;

namespace ProductReviewAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IDataRepository<Product> _productRepository;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(
        IDataRepository<Product> productRepository,
        ILogger<ProductsController> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        private double CalculateAverageRating(IEnumerable<Review> reviews)
        {
            if (reviews.Any())
            {
                return reviews.Average(r => r.Rating);
            }
            return 0;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                Expression<Func<Product, object>>[] includes = {
                    p => p.Images,
                    p => p.Reviews,
                    p => p.SellerProducts,
                    p => p.Category
                };

                var products = await _productRepository.GetAllAsync(null, includes);

                var productDTOs = new List<ProductDtoWithAvgRating>();

                foreach (var product in products)
                {
                    var productDTO = new ProductDtoWithAvgRating
                    {
                        ProductId = product.ProductId,
                        Name = product.Name,
                        Description = product.Description,
                        CategoryId = product.CategoryId,
                        // Calculate average rating
                        AverageRating = CalculateAverageRating(product.Reviews)
                    };

                    // Map Category
                    productDTO.Category = new CategoryDto
                    {
                        CategoryId = product.Category.CategoryId,
                        Name = product.Category.Name
                    };

                    // Map SellerProducts
                    productDTO.SellerProducts = product.SellerProducts
                    .OrderBy(sellerProduct => sellerProduct.Price)
                    .Select(sellerProduct => new SellerProductDTO
                    {
                        SellerProductId = sellerProduct.SellerProductId,
                        SellerId = sellerProduct.SellerId,
                        ProductId = sellerProduct.ProductId,
                        Price = sellerProduct.Price
                    })
                    .ToList();

                    // Map Reviews
                    productDTO.Reviews = product.Reviews.Select(review => new ReviewDto
                    {
                        ReviewId = review.ReviewId,
                        Rating = review.Rating,
                        Comment = review.Comment,
                        UserId = review.UserId
                    }).ToList();

                    // Map Images
                    productDTO.Images = product.Images.Select(image => new ProductImageDto
                    {
                        ImageId = image.ImageId,
                        Url = image.Url
                    }).ToList();

                    productDTOs.Add(productDTO);
                }

                return Ok(productDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving product details: {ex}");
                return StatusCode(500, "An error occurred while processing your request.");
            }

        }

        [HttpGet("{productId}", Name = "GetProductById")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<ProductDtoWithAvgRating>> GetProductById(int productId)
        {
            try
            {
                Expression<Func<Product, object>>[] includes = {
                    p => p.Images,
                    p => p.Reviews,
                    p => p.SellerProducts,
                    p => p.Category
                };
            // Load product with related data using Include
            var product = await _productRepository.GetByIdAsync(productId, includes );

            if (product == null)
            {
                return NotFound();
            }

            var productDTO = new ProductDtoWithAvgRating
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                CategoryId = product.CategoryId,
                // Calculate average rating
                AverageRating = CalculateAverageRating(product.Reviews)
            };

            // Map Category
            productDTO.Category = new CategoryDto
            {
                CategoryId = product.Category.CategoryId,
                Name = product.Category.Name
            };

            // Map SellerProducts
            productDTO.SellerProducts = product.SellerProducts.Select(sellerProduct => new SellerProductDTO
            {
                SellerProductId = sellerProduct.SellerProductId,
                SellerId = sellerProduct.SellerId,
                ProductId = sellerProduct.ProductId,
                Price = sellerProduct.Price
            }).ToList();

            // Map Reviews
            productDTO.Reviews = product.Reviews.Select(review => new ReviewDto
            {
                ReviewId = review.ReviewId,
                Rating = review.Rating,
                Comment = review.Comment,
                UserId = review.UserId
            }).ToList();

            // Map Images
            productDTO.Images = product.Images.Select(image => new ProductImageDto
            {
                ImageId = image.ImageId,
                Url = image.Url
            }).ToList();

            return Ok(productDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving product details: {ex}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CompareProducts([FromBody] List<int> productIds)
        {
            try
            {
                if (productIds == null || productIds.Count == 0)
                {
                    return BadRequest("Product IDs are required.");
                } else if (productIds.Count < 2)
                {
                    return BadRequest("Minimum two product IDs are required for compare.");
                }

                Expression<Func<Product, object>>[] includes = {
                    p => p.Images,
                    p => p.Reviews,
                    p => p.SellerProducts,
                    p => p.Category
                };

                // Retrieve the products based on the provided IDs
                var products = await _productRepository.GetAllAsync(
                    p => productIds.Contains(p.ProductId),
                    includes);

                if (productIds.Count != products.Count())
                {
                    return BadRequest("One or more product/s are not found.");
                }

                // Check if all products have the same CategoryId
                int categoryId = products.FirstOrDefault()?.CategoryId ?? -1;
                if (products.Any(p => p.CategoryId != categoryId))
                {
                    return BadRequest("All products must have the same CategoryId.");
                }

                var productDTOs = new List<ProductDtoWithAvgRating>();

                foreach (var product in products)
                {
                    var productDTO = new ProductDtoWithAvgRating
                    {
                        ProductId = product.ProductId,
                        Name = product.Name,
                        Description = product.Description,
                        CategoryId = product.CategoryId,
                        AverageRating = CalculateAverageRating(product.Reviews)
                    };

                    productDTO.Category = new CategoryDto
                    {
                        CategoryId = product.Category.CategoryId,
                        Name = product.Category.Name
                    };

                    productDTO.SellerProducts = product.SellerProducts
                        .OrderBy(sellerProduct => sellerProduct.Price)
                        .Select(sellerProduct => new SellerProductDTO
                        {
                            SellerProductId = sellerProduct.SellerProductId,
                            SellerId = sellerProduct.SellerId,
                            ProductId = sellerProduct.ProductId,
                            Price = sellerProduct.Price
                        })
                        .ToList();

                    productDTO.Reviews = product.Reviews.Select(review => new ReviewDto
                    {
                        ReviewId = review.ReviewId,
                        Rating = review.Rating,
                        Comment = review.Comment,
                        UserId = review.UserId
                    }).ToList();

                    productDTO.Images = product.Images.Select(image => new ProductImageDto
                    {
                        ImageId = image.ImageId,
                        Url = image.Url
                    }).ToList();

                    productDTOs.Add(productDTO);
                }

                return Ok(productDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving products by IDs: {ex}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("category/{categoryId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<IEnumerable<ProductDtoWithAvgRating>>> GetProductsByCategory(int categoryId)
        {
            try
            {
                var products = await _productRepository.GetAllAsync(
                    p => p.CategoryId == categoryId,
                    p => p.Images,
                    p => p.Reviews,
                    p => p.SellerProducts,
                    p => p.Category);

                var productDTOs = new List<ProductDtoWithAvgRating>();

                foreach (var product in products)
                {
                    var productDTO = new ProductDtoWithAvgRating
                    {
                        ProductId = product.ProductId,
                        Name = product.Name,
                        Description = product.Description,
                        CategoryId = product.CategoryId,
                        Category = new CategoryDto
                        {
                            CategoryId = product.Category.CategoryId,
                            Name = product.Category.Name
                        },
                        Images = product.Images.Select(image => new ProductImageDto
                        {
                            ImageId = image.ImageId,
                            Url = image.Url
                        }).ToList(),
                        Reviews = product.Reviews.Select(review => new ReviewDto
                        {
                            ReviewId = review.ReviewId,
                            Rating = review.Rating,
                            Comment = review.Comment,
                            UserId = review.UserId
                        }).ToList(),
                        AverageRating = CalculateAverageRating(product.Reviews),
                        SellerProducts = product.SellerProducts
                            .OrderBy(sellerProduct => sellerProduct.Price)
                            .Select(sellerProduct => new SellerProductDTO
                            {
                                SellerProductId = sellerProduct.SellerProductId,
                                SellerId = sellerProduct.SellerId,
                                ProductId = sellerProduct.ProductId,
                                Price = sellerProduct.Price
                            }).ToList()
                    };

                    productDTOs.Add(productDTO);
                }

                return Ok(productDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving products by category: {ex}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "0")]
        public async Task<ActionResult<ProductCreateDTO>> AddProduct(ProductCreateDTO productCreateDto)
        {
            try
            {
                // Map the product DTO to the Product entity
                var product = new Product
                {
                    Name = productCreateDto.Name,
                    Description = productCreateDto.Description,
                    CategoryId = productCreateDto.CategoryId
                };

                await _productRepository.AddAsync(product);

                var createdProductDto = new ProductCreateDTO
                {
                    Name = product.Name,
                    Description = product.Description,
                    CategoryId = product.CategoryId
                };

                var routeValues = new { productId = product.ProductId };

                return CreatedAtAction(nameof(GetProductById), routeValues, createdProductDto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding product: {ex}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPut("{productId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "0")]
        public async Task<IActionResult> UpdateProduct(int productId, Product product)
        {
            if (productId != product.ProductId)
            {
                return BadRequest();
            }

            await _productRepository.UpdateAsync(product);
            return NoContent();
        }

        [HttpPut("{productId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "0")]
        public async Task<ActionResult<ProductDto>> UpdateProductImages(int productId, List<ProductImageDto> images)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(productId);

                if (product == null)
                {
                    return NotFound($"Product with ID {productId} not found.");
                }

                // Clear existing images for the product
                product.Images.Clear();

                // Map and add the new images
                foreach (var imageDto in images)
                {
                    var image = new ProductImage
                    {
                        Url = imageDto.Url
                    };

                    product.Images.Add(image);
                }

                await _productRepository.UpdateAsync(product);

                var productDto = new ProductDto
                {
                    ProductId = product.ProductId,
                    Name = product.Name,
                    Description = product.Description,
                    CategoryId = product.CategoryId
                };

                return Ok(productDto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating product images: {ex}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPut("{productId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "0")]
        public async Task<ActionResult<ProductUpdateDTO>> AssignSellerToProduct(int productId, SellerProductDTO sellerProductDto)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(productId);

                if (product == null)
                {
                    return NotFound($"Product with ID {productId} not found.");
                }

                // Initialize the SellerProducts collection if it's null
                if (product.SellerProducts == null)
                {
                    product.SellerProducts = new List<SellerProduct>();
                }

                // Map seller product DTO to SellerProduct entity
                var sellerProduct = new SellerProduct
                {
                    SellerId = sellerProductDto.SellerId,
                    Price = sellerProductDto.Price,
                    ProductId = product.ProductId
                };

                product.SellerProducts.Add(sellerProduct);

                await _productRepository.UpdateAsync(product);

                var productUpdateDto = new ProductUpdateDTO
                {
                    Name = product.Name,
                    Description = product.Description,
                    CategoryId = product.CategoryId
                };

                return Ok(productUpdateDto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error assigning seller to product: {ex}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPut("{productId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<ReviewDto>> AddReview(int productId, ReviewCreateDTO reviewDto)
        {
            try
            {
                // Get the current user's ID
                int userId = int.Parse(User.FindFirst("Id").Value);

                Expression<Func<Product, object>>[] includes = {
                    p => p.Images,
                    p => p.Reviews,
                    p => p.SellerProducts,
                    p => p.Category
                };
                // Load product with related data using Include
                var product = await _productRepository.GetByIdAsync(productId, includes);

                if (product == null)
                {
                    return NotFound($"Product with ID {productId} not found.");
                }


                // Initialize the SellerProducts collection if it's null
                if (product.Reviews == null)
                {
                    product.Reviews = new List<Review>();
                }

                // Check if the user has already reviewed this product
                var existingReview = product.Reviews.Where(r => r.UserId == userId).ToList();

                if (existingReview.Any())
                {
                    return BadRequest("You have already reviewed this product.");
                }

                // Map the review to the review model
                var review = new Review
                {
                    UserId = userId,
                    ProductId = productId,
                    Rating = reviewDto.Rating,
                    Comment = reviewDto.Comment
                };

                product.Reviews.Add(review);

                await _productRepository.UpdateAsync(product);

                var productUpdateDto = new ProductUpdateDTO
                {
                    Name = product.Name,
                    Description = product.Description,
                    CategoryId = product.CategoryId
                };

                return Ok(productUpdateDto);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding review: {ex}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        //TODO:  Instead of Delete from db add isDeleted flag and change it to true
        [HttpDelete("{productId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "0")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            await _productRepository.DeleteAsync(product);
            return NoContent();
        }

    }
}
