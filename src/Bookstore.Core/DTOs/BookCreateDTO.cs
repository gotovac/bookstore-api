using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Core.DTOs
{
    public class BookCreateDTO
    {
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        [Required]
        [MinLength(1)]
        public List<long> AuthorIds { get; set; } = [];
        [Required]
        [MinLength(1)]
        public List<long> GenreIds { get; set; } = [];
    }
}
