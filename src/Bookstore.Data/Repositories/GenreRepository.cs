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
    public class GenreRepository : BaseRepository<Genre>, IGenreRepository
    {
        public GenreRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<Genre?> GetByNameAsync(string name)
        {
            var normalizedName = Genre.NormalizeName(name);

            return await _dbSet
                .FirstOrDefaultAsync(g => g.NormalizedName == normalizedName);
        }

        public async Task<Dictionary<string, Genre>> GetAllNormalizedAsync()
        {
            return await _dbSet
                .ToDictionaryAsync(g => g.NormalizedName, g => g);
        }
    }
}
