using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Core.DTOs
{
    public class ReviewCreateDTO
    {
        [Required]
        [Range(1,5)]
        public int Rating { get; set; }
        public string Description { get; set; } = string.Empty;
        [Required]
        [Range(1, long.MaxValue)]
        public long BookId { get; set; }
    }
}
