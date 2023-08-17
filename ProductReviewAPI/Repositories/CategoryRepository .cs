using Microsoft.EntityFrameworkCore;
using ProductReviewAPI.Models;
using System.Linq.Expressions;

namespace ProductReviewAPI.Repositories
{
    // CategoryRepository.cs
    public class CategoryRepository : IDataRepository<Category>
    {
        private readonly ReviewDBContext _context;
        private readonly ILogger<CategoryRepository> _logger;

        public CategoryRepository(ReviewDBContext dbContext, ILogger<CategoryRepository> logger)
        {
            _context = dbContext;
            _logger = logger;
        }

        public async Task<Category> GetByIdAsync(int id, params Expression<Func<Category, object>>[] includes)
        {
            _logger.LogInformation($"Getting category with ID: {id}");
            var query = _context.Categories.AsQueryable();

            query = includes.Aggregate(query, (current, include) => current.Include(include));

            return await query.FirstOrDefaultAsync(category => category.CategoryId == id);
        }

        public async Task<IEnumerable<Category>> GetAllAsync(Expression<Func<Category, bool>> filter = null, params Expression<Func<Category, object>>[] includes)
        {
            var query = _context.Categories.AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            query = includes.Aggregate(query, (current, include) => current.Include(include));

            return await query.ToListAsync();
        }

        public async Task AddAsync(Category entity)
        {
            _logger.LogInformation($"Adding category: {entity.Name}");
            await _context.Categories.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category entity)
        {
            _logger.LogInformation($"Update category: {entity.CategoryId} {entity.Name}");
            _context.Categories.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Category entity)
        {
            _logger.LogInformation($"Delete category: {entity.CategoryId} {entity.Name}");
            _context.Categories.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public Task<IEnumerable<Category>> GetAllListAsync()
        {
            throw new NotImplementedException();
        }
    }

}
