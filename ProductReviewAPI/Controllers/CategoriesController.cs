using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using ProductReviewAPI.Models;
using ProductReviewAPI.Repositories;

namespace ProductReviewAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IDataRepository<Category> _categoryRepository;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(
            IDataRepository<Category> categoryRepository,
            ILogger<CategoriesController> logger)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAllCategories()
        {
            _logger.LogInformation("Getting all categories");
            var categories = await _categoryRepository.GetAllAsync();
            return Ok(categories);
        }

        [HttpGet("{categoryId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetCategoryById(int categoryId)
        {
            _logger.LogInformation($"Getting category for {categoryId}");
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
            {
                _logger.LogWarning($"Category not found for {categoryId}");
                return NotFound();
            }
            return Ok(category);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "0")]
        public async Task<IActionResult> AddCategory(Category category)
        {
            await _categoryRepository.AddAsync(category);
            return CreatedAtAction(nameof(GetCategoryById), new { categoryId = category.CategoryId }, category);
        }

        [HttpPut("{categoryId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "0")]
        public async Task<IActionResult> UpdateCategory(int categoryId, Category category)
        {
            if (categoryId != category.CategoryId)
            {
                _logger.LogWarning($"Category ID: {categoryId} not match with CategoryId {category.CategoryId}");
                return BadRequest();
            }

            await _categoryRepository.UpdateAsync(category);
            return NoContent();
        }

        //TODO: Instead of Delete from db add isDeleted flag and change it to true
        //[HttpDelete("{id}")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "0")]
        //public async Task<IActionResult> DeleteCategory(int id)
        //{
        //    var category = await _categoryRepository.GetByIdAsync(id);
        //    if (category == null)
        //    {
        //        _logger.LogWarning($"Category not found for {id}");
        //        return NotFound();
        //    }

        //    await _categoryRepository.DeleteAsync(category);
        //    return NoContent();
        //}
    }
}
