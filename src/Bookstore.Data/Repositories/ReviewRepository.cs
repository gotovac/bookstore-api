using Bookstore.Core.Interfaces.Repositories;
using Bookstore.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Data.Repositories
{
    public class ReviewRepository : BaseRepository<Review>, IReviewRepository
    {
        public ReviewRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<Review>> GetByBookIdAsync(long bookId)
        {
            return await _dbSet
                .Where(r => r.BookId == bookId)
                .ToListAsync();
        }
        public async Task<double> GetAverageRatingForBookAsync(long bookId)
        {
            var ratings = await _dbSet
                .Where(r => r.BookId == bookId)
                .Select(r => r.Rating)
                .ToListAsync();

            if (ratings.Count == 0)
                return 0.0;
            return ratings.Average();
        }
    }
}
