using Microsoft.EntityFrameworkCore;
using ProductReviewAPI.Models;
using System.Linq.Expressions;

namespace ProductReviewAPI.Repositories
{
    public class UserRepository: IDataRepository<User>
    {
        private ReviewDBContext _context;

        public UserRepository(ReviewDBContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<User> GetByIdAsync(int id, params Expression<Func<User, object>>[] includes)
        {
            var query = _context.Users.AsQueryable();

            query = includes.Aggregate(query, (current, include) => current.Include(include));

            return await query.FirstOrDefaultAsync(user => user.UserId == id);
        }

        public async Task<IEnumerable<User>> GetAllAsync(Expression<Func<User, bool>> filter = null, params Expression<Func<User, object>>[] includes)
        {
            var query = _context.Users.AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            query = includes.Aggregate(query, (current, include) => current.Include(include));

            return await query.ToListAsync();
        }

        public async Task AddAsync(User entity)
        {
            await _context.Users.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User entity)
        {
            _context.Users.Update(entity);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(User entity)
        {
            _context.Users.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
