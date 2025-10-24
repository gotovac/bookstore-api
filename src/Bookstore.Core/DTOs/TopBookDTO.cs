using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Core.DTOs
{
    public class TopBookDTO
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public double AverageRating { get; set; }
    }
}
