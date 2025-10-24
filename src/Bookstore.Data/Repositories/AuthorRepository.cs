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
    public class AuthorRepository : BaseRepository<Author>, IAuthorRepository
    {
        public AuthorRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<Author?> GetByNameAsync(string name)
        {
            var normalizedName = Author.NormalizeName(name);

            return await _dbSet
                .FirstOrDefaultAsync(a => a.NormalizedName == normalizedName);
        }

        public async Task<Dictionary<string, Author>> GetAllNormalizedAsync()
        {
            return await _dbSet
                .ToDictionaryAsync(a => a.NormalizedName, a => a);
        }
    }
}
