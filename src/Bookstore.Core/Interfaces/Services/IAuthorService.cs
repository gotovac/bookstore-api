using Bookstore.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Core.Interfaces.Services
{
    public interface IAuthorService
    {
        Task<IEnumerable<AuthorDTO>> GetAllAuthorsAsync();
        Task<AuthorDTO?> GetByIdAsync(long id);
        Task<long> AddAuthorAsync(AuthorCreateDTO dto);
        Task DeleteAuthorAsync(long id);
    }
}
