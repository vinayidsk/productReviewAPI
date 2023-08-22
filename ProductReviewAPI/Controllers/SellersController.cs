using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductReviewAPI.Models;
using ProductReviewAPI.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ProductReviewAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SellersController : ControllerBase
    {
        private readonly IDataRepository<Seller> _sellerRepository;
        private readonly ILogger<SellersController> _logger;

        public SellersController(IDataRepository<Seller> sellerRepository, ILogger<SellersController> logger)
        {
            _sellerRepository = sellerRepository;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAllSellers()
        {
            var sellers = await _sellerRepository.GetAllAsync();
            return Ok(sellers);
        }

        [HttpGet("{sellerId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetSellerById(int sellerId)
        {
            var seller = await _sellerRepository.GetByIdAsync(sellerId);
            if (seller == null)
            {
                return NotFound();
            }
            return Ok(seller);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "0")]
        public async Task<IActionResult> CreateSeller(Seller seller)
        {
            if (seller == null)
            {
                return BadRequest("Seller data is invalid.");
            }

            await _sellerRepository.AddAsync(seller);

            return CreatedAtAction(nameof(GetSellerById), new { sellerId = seller.SellerId }, seller);
        }

        [HttpPut("{sellerId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "0")]
        public async Task<IActionResult> UpdateSeller(int sellerId, Seller updatedSeller)
        {
            var existingSeller = await _sellerRepository.GetByIdAsync(sellerId);
            if (existingSeller == null)
            {
                return NotFound();
            }

            existingSeller.Name = updatedSeller.Name;

            await _sellerRepository.UpdateAsync(existingSeller);

            return NoContent();
        }

        //TODO:  Instead of Delete from db add isDeleted flag and change it to true
        //[HttpDelete("{id}")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "0")]
        //public async Task<IActionResult> DeleteSeller(int id)
        //{
        //    var existingSeller = await _sellerRepository.GetByIdAsync(id);
        //    if (existingSeller == null)
        //    {
        //        return NotFound();
        //    }

        //    await _sellerRepository.DeleteAsync(existingSeller);

        //    return NoContent();
        //}
    }
}
