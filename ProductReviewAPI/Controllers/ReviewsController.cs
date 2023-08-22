using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductReviewAPI.Dtos;
using ProductReviewAPI.Models;
using ProductReviewAPI.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ProductReviewAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IDataRepository<Review> _reviewRepository;
        private readonly ILogger<ReviewsController> _logger;

        public ReviewsController(IDataRepository<Review> reviewRepository, ILogger<ReviewsController> logger)
        {
            _reviewRepository = reviewRepository;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "0")]
        public async Task<IActionResult> GetAllReviews()
        {
            var reviews = await _reviewRepository.GetAllAsync();
            return Ok(reviews);
        }

        [HttpGet("{reviewId}", Name = "GetReviewById")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetReviewById(int reviewId)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);
            if (review == null || review?.IsDeleted == true)
            {
                return NotFound("Review not found or deleted");
            }
            return Ok(review);
        }

        [HttpPut("{reviewId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "0")]
        public async Task<IActionResult> UpdateReview(int reviewId, Review updatedReview)
        {
            var existingReview = await _reviewRepository.GetByIdAsync(reviewId);
            if (existingReview == null || existingReview?.IsDeleted == true)
            {
                return NotFound("Review not found or deleted");
            }

            existingReview.Rating = updatedReview.Rating;
            existingReview.Comment = updatedReview.Comment;
            existingReview.UserId = updatedReview.UserId;
            existingReview.ProductId = updatedReview.ProductId;

            await _reviewRepository.UpdateAsync(existingReview);

            return NoContent();
        }

        //TODO:  Instead of Delete from db add isDeleted flag and change it to true
        [HttpDelete("{reviewId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "0")]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            var existingReview = await _reviewRepository.GetByIdAsync(reviewId);
            if (existingReview == null || existingReview?.IsDeleted == true)
            {
                return NotFound("Review not found or deleted");
            }

            await _reviewRepository.DeleteAsync(existingReview);

            return NoContent();
        }
    }
}
