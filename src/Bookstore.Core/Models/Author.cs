using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Core.Models
{
    public class Author
    {
        public long Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public int BirthYear { get; set; }

        public ICollection<Book> Books { get; set; } = new List<Book>();

        public string NormalizedName { get; set; } = string.Empty;
        public static string NormalizeName(string name)
        {
            return (name ?? string.Empty).Trim().ToLowerInvariant();
        }
    }
}
