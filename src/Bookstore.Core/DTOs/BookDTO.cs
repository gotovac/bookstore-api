using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Core.DTOs
{
    public class BookDTO
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public List<string> Authors { get; set; } = [];
        public List<string> Genres { get; set; } = [];
        public double AverageRating { get; set; }
    }
}
