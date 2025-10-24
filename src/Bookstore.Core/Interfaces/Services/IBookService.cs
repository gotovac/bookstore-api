using Bookstore.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Core.Interfaces.Services
{
    public interface IBookService
    {
        Task<IEnumerable<BookDTO>> GetAllBooksAsync();
        Task<IEnumerable<BookDTO>> GetTopRatedBooksAsync(int count = 10);
        Task<BookDTO?> GetByIdAsync(long id);
        Task<long> AddBookAsync(BookCreateDTO dto, bool failOnDuplicate);
        Task UpdatePriceAsync(long id, decimal newPrice);
        Task DeleteBookAsync(long id);
        Task ImportBooksAsync(IEnumerable<BookImportDTO> importBooks);
    }
}
