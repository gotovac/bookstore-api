using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Core.DTOs
{
    public class ReviewDTO
    {
        public long Id { get; set; }
        public int Rating { get; set; }
        public string Description { get; set; } = string.Empty;
        public long BookId { get; set; }
    }
}
