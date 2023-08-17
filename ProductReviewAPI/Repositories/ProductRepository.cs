using Microsoft.EntityFrameworkCore;
using ProductReviewAPI.Models;
using System.Linq.Expressions;

namespace ProductReviewAPI.Repositories
{
    public class ProductRepository : IDataRepository<Product>
    {
        private ReviewDBContext _context;

        public ProductRepository(ReviewDBContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<IEnumerable<Product>> GetAllAsync(Expression<Func<Product, bool>> filter = null, params Expression<Func<Product, object>>[] includes)
        {
            IQueryable<Product> query = _context.Products;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync();
        }

        public async Task<Product> GetByIdAsync(int id, params Expression<Func<Product, object>>[] includes)
        {
            IQueryable<Product> query = _context.Products.Where(p => p.ProductId == id);

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task AddAsync(Product entity)
        {
            await _context.Products.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product entity)
        {
            _context.Products.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Product entity)
        {
            _context.Products.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
