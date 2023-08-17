using Microsoft.EntityFrameworkCore;
using ProductReviewAPI.Models;
using System.Linq.Expressions;

namespace ProductReviewAPI.Repositories
{
    public class ReviewRepository : IDataRepository<Review>
    {
        private readonly ReviewDBContext _context;
        public ReviewRepository(ReviewDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Review>> GetAllAsync(Expression<Func<Review, bool>> filter = null, params Expression<Func<Review, object>>[] includes)
        {
            var query = _context.Reviews.AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            query = includes.Aggregate(query, (current, include) => current.Include(include));

            return await query.ToListAsync();
        }

        public async Task<Review> GetByIdAsync(int id, params Expression<Func<Review, object>>[] includes)
        {
            var query = _context.Reviews.AsQueryable();

            query = includes.Aggregate(query, (current, include) => current.Include(include));

            return await query.FirstOrDefaultAsync(review => review.ReviewId == id);
        }

        public async Task AddAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Review review)
        {
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Review review)
        {
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
        }
    }
}
