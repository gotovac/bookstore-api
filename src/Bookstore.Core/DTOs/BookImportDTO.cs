using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Core.DTOs
{
    public class BookImportDTO
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public List<AuthorCreateDTO> Authors { get; set; } = [];
        public List<GenreCreateDTO> Genres { get; set; } = [];
    }
}
