using Bookstore.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Core.Interfaces.Services
{
    public interface IGenreService
    {
        Task<IEnumerable<GenreDTO>> GetAllGenresAsync();
        Task<GenreDTO?> GetByIdAsync(long id);
        Task<long> AddGenreAsync(GenreCreateDTO dto);
        Task DeleteGenreAsync(long id);
    }
}
