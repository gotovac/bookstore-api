using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Core.Models
{
    public class Book
    {
        public long Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }

        [MinLength(1)]
        public ICollection<Author> Authors { get; set; } = new List<Author>();
        [MinLength(1)]
        public ICollection<Genre> Genres { get; set; } = new List<Genre>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();

        [NotMapped]
        public double AverageRating { get; set; }

        public string NormalizedTitle { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public static string NormalizeTitle(string title)
        {
            return (title ?? string.Empty).Trim().ToLowerInvariant();
        }
    }
}
