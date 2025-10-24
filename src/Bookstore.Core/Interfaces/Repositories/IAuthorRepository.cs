using Bookstore.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Core.Interfaces.Repositories
{
    public interface IAuthorRepository : IBaseRepository<Author>
    {
        Task<Author?> GetByNameAsync(string name);
        Task<Dictionary<string, Author>> GetAllNormalizedAsync();
    }
}
