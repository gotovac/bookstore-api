using Bookstore.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Core.Interfaces.Repositories
{
    public interface IBookRepository : IBaseRepository<Book>
    {
        IQueryable<Book> GetAllQueryable();
        Task<IEnumerable<Book>> GetTopRatedBooksAsync(int count = 10);
        Task<Book?> GetByTitleAsync(string title);
        Task<HashSet<string>> GetAllNormalizedTitlesAsync();
    }
}
