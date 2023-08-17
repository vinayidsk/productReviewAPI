﻿using Microsoft.AspNetCore.Authorization;
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAllReviews()
        {
            var reviews = await _reviewRepository.GetAllAsync();
            return Ok(reviews);
        }

        [HttpGet("{ReviewId}", Name = "GetReviewById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetReviewById(int ReviewId)
        {
            var review = await _reviewRepository.GetByIdAsync(ReviewId);
            if (review == null)
            {
                return NotFound();
            }
            return Ok(review);
        }

        [HttpGet("product/{productId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetReviewsForProduct(int productId)
        {
            var reviewsForProduct = await _reviewRepository.GetAllAsync(r => r.ProductId == productId);
            return Ok(reviewsForProduct);
        }

        [HttpGet("product/{productId}/average-rating")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAverageRatingForProduct(int productId)
        {
            var reviewsForProduct = await _reviewRepository.GetAllAsync(r => r.ProductId == productId);

            if (!reviewsForProduct.Any())
            {
                return NotFound();
            }

            var averageRating = reviewsForProduct.Average(r => r.Rating);
            return Ok(averageRating);
        }

        [HttpPost("{productId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<ReviewDto>> CreateReview(int productId, ReviewCreateDTO reviewDto)
        {
            try
            {
                // Get the current user's ID
                int userId = int.Parse(User.FindFirst("Id").Value);

                // Check if the user has already reviewed this product
                var existingReview = await _reviewRepository.GetAllAsync(
                    r => r.ProductId == productId && r.UserId == userId);

                if (existingReview.Any())
                {
                    return BadRequest("You have already reviewed this product.");
                }

                // Map the review DTO to the review model
                var review = new Review
                {
                    UserId = userId,
                    ProductId = productId,
                    Rating = reviewDto.Rating,
                    Comment = reviewDto.Comment
                };

                // Add the review to the database
                await _reviewRepository.AddAsync(review);

                // Map the added review back to DTO
                var addedReviewDto = new ReviewDto
                {
                    ReviewId = review.ReviewId,
                    UserId = review.UserId,
                    ProductId = review.ProductId,
                    Rating = review.Rating,
                    Comment = review.Comment
                };

                var routeValues = new { ReviewId = addedReviewDto.ReviewId };

                return CreatedAtAction(nameof(GetReviewById), routeValues, addedReviewDto);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding review: {ex}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "0")]
        public async Task<IActionResult> UpdateReview(int id, Review updatedReview)
        {
            var existingReview = await _reviewRepository.GetByIdAsync(id);
            if (existingReview == null)
            {
                return NotFound();
            }

            existingReview.Rating = updatedReview.Rating;
            existingReview.Comment = updatedReview.Comment;
            existingReview.UserId = updatedReview.UserId;
            existingReview.ProductId = updatedReview.ProductId;

            await _reviewRepository.UpdateAsync(existingReview);

            return NoContent();
        }

        //TODO:  Instead of Delete from db add isDeleted flag and change it to true
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "0")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var existingReview = await _reviewRepository.GetByIdAsync(id);
            if (existingReview == null)
            {
                return NotFound();
            }

            await _reviewRepository.DeleteAsync(existingReview);

            return NoContent();
        }
    }
}