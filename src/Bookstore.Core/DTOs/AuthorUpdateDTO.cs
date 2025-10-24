using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Core.DTOs
{
    public class AuthorUpdateDTO
    {
        public string Name { get; set; } = string.Empty;
        public int BirthYear { get; set; }
    }
}
