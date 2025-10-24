using Bookstore.Core.DTOs;
using Bookstore.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Jobs.Services
{
    public static class BookImportService
    {
        private static readonly Random _random = new();

        public static List<BookImportDTO> GenerateBooks(int minCount = 100000, int maxCount = 200000)
        {
            int count = _random.Next(minCount, maxCount + 1);
            var books = new List<BookImportDTO>(count);

            for (int i = 0; i < count; i++)
            {
                books.Add(new BookImportDTO
                {
                    Title = $"Book Title {_random.Next(1000000)}",
                    Price = Math.Round((decimal)(_random.NextDouble() * 100), 2),
                    Authors = new List<AuthorCreateDTO>
                    {
                        new AuthorCreateDTO { Name = $"Author {_random.Next(1000000)}", BirthYear = _random.Next(1900, 2000) }
                    },
                    Genres = new List<GenreCreateDTO>
                    {
                        new GenreCreateDTO { Name = $"Genre {_random.Next(1000)}" }
                    }
                });
            }

            return books;
        }
    }
}
