using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Core.Models
{
    public class Review
    {
        public long Id { get; set; }
        [Required]
        public long BookId { get; set; }
        public Book Book { get; set; } = null!;
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
