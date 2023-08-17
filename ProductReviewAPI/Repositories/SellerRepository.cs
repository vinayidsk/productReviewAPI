using Microsoft.EntityFrameworkCore;
using ProductReviewAPI.Models;
using System.Linq.Expressions;

namespace ProductReviewAPI.Repositories
{
    public class SellerRepository : IDataRepository<Seller>
    {
        private readonly ReviewDBContext _context;

        public SellerRepository(ReviewDBContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<Seller> GetByIdAsync(int id, params Expression<Func<Seller, object>>[] includes)
        {
            var query = _context.Sellers.AsQueryable();

            query = includes.Aggregate(query, (current, include) => current.Include(include));

            return await query.FirstOrDefaultAsync(seller => seller.SellerId == id);
        }

        public async Task<IEnumerable<Seller>> GetAllAsync(Expression<Func<Seller, bool>> filter = null, params Expression<Func<Seller, object>>[] includes)
        {
            var query = _context.Sellers.AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            query = includes.Aggregate(query, (current, include) => current.Include(include));

            return await query.ToListAsync();
        }

        public async Task AddAsync(Seller entity)
        {
            await _context.Sellers.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Seller entity)
        {
            _context.Sellers.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Seller entity)
        {
            _context.Sellers.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
