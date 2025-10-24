using Bookstore.Core.Interfaces.Repositories;
using Bookstore.Core.Models;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Data.Repositories
{
    public class BookRepository : BaseRepository<Book>, IBookRepository
    {
        public BookRepository(AppDbContext context) : base(context)
        {
        }

        public IQueryable<Book> GetAllQueryable()
        {
            return _dbSet.AsNoTracking();
        }

        public override async Task<Book?> GetByIdAsync(long id)
        {
            return await _dbSet
                .Include(b => b.Authors)
                .Include(b => b.Genres)
                .Include(b => b.Reviews)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Book>> GetTopRatedBooksAsync(int count = 10)
        {
            // Dapper solution for top 10 books
            var connection = _context.Database.GetDbConnection();
            var sqlBooks = @"
                SELECT TOP (@Count)
                    b.Id,
                    b.Title,
                    b.Price,
                    AVG(CAST(r.Rating AS FLOAT)) AS AverageRating
                FROM Books b
                LEFT JOIN Reviews r ON b.Id = r.BookId
                GROUP BY b.Id, b.Title, b.Price
                ORDER BY AverageRating DESC;";

            var topBooks = (await connection.QueryAsync<Book>(sqlBooks, new { Count = count })).ToList();

            if (!topBooks.Any())
                return topBooks;

            var bookIds = topBooks.Select(b => b.Id).ToList();

            var sqlAuthors = @"
                SELECT ab.BooksId, a.Id, a.Name, a.BirthYear
                FROM AuthorBook ab
                JOIN Authors a ON a.Id = ab.AuthorsId
                WHERE ab.BooksId IN @BookIds;";

            var authorRows = await connection.QueryAsync<(long BookId, long Id, string Name, int BirthYear)>(sqlAuthors, new { BookIds = bookIds });

            var sqlGenres = @"
                SELECT bg.BooksId, g.Id, g.Name
                FROM BookGenre bg
                JOIN Genres g ON g.Id = bg.GenresId
                WHERE bg.BooksId IN @BookIds;";

            var genreRows = await connection.QueryAsync<(long BookId, long Id, string Name)>(sqlGenres, new { BookIds = bookIds });

            foreach (var book in topBooks)
            {
                book.Authors = authorRows
                    .Where(a => a.BookId == book.Id)
                    .Select(a => new Author { Id = a.Id, Name = a.Name, BirthYear = a.BirthYear })
                    .ToList();

                book.Genres = genreRows
                    .Where(g => g.BookId == book.Id)
                    .Select(g => new Genre { Id = g.Id, Name = g.Name })
                    .ToList();
            }

            return topBooks;
        }

        public async Task<Book?> GetByTitleAsync(string title)
        {
            var normalizedTitle = Book.NormalizeTitle(title);

            return await _dbSet
                .Include(b => b.Authors)
                .Include(b => b.Genres)
                .FirstOrDefaultAsync(b => b.NormalizedTitle == normalizedTitle);
        }

        public async Task<HashSet<string>> GetAllNormalizedTitlesAsync()
        {
            return (await _dbSet
                .AsNoTracking()
                .Select(b => b.NormalizedTitle)
                .ToListAsync())
                .ToHashSet();
        }
    }
}
