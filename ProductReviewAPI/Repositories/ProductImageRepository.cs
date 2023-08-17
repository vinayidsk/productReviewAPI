using Microsoft.EntityFrameworkCore;
using ProductReviewAPI.Models;
using System.Linq.Expressions;

namespace ProductReviewAPI.Repositories
{
    public class ProductImageRepository : IDataRepository<ProductImage>
    {
        private readonly ReviewDBContext _context;

        public ProductImageRepository(ReviewDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductImage>> GetAllAsync(Expression<Func<ProductImage, bool>> filter = null, params Expression<Func<ProductImage, object>>[] includes)
        {
            var query = _context.ProductImages.AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            query = includes.Aggregate(query, (current, include) => current.Include(include));

            return await query.ToListAsync();
        }

        public async Task<ProductImage> GetByIdAsync(int id, params Expression<Func<ProductImage, object>>[] includes)
        {
            var query = _context.ProductImages.AsQueryable();

            query = includes.Aggregate(query, (current, include) => current.Include(include));

            return await query.FirstOrDefaultAsync(productImage => productImage.ImageId == id);
        }

        public async Task AddAsync(ProductImage entity)
        {
            await _context.ProductImages.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProductImage entity)
        {
            _context.ProductImages.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ProductImage entity)
        {
            _context.ProductImages.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
