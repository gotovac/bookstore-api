using AutoMapper;
using AutoMapper.QueryableExtensions;
using Bookstore.Core.DTOs;
using Bookstore.Core.Interfaces.Repositories;
using Bookstore.Core.Interfaces.Services;
using Bookstore.Core.Models;
using Bookstore.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BookService> _logger;

        public BookService(IBookRepository bookRepository, IAuthorRepository authorRepository, IGenreRepository genreRepository, IMapper mapper, ILogger<BookService> logger)
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _genreRepository = genreRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<BookDTO>> GetAllBooksAsync()
        {
            var books = await _bookRepository.GetAllQueryable()
                .ProjectTo<BookDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return books;
        }

        public async Task<IEnumerable<BookDTO>> GetTopRatedBooksAsync(int count = 10)
        {
            var topBooks = await _bookRepository.GetTopRatedBooksAsync(count);
            var mappedTopBooks = _mapper.Map<IEnumerable<BookDTO>>(topBooks);

            return mappedTopBooks;
        }
        public async Task<BookDTO?> GetByIdAsync(long id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            return _mapper.Map<BookDTO?>(book);
        }
        public async Task<long> AddBookAsync(BookCreateDTO dto, bool failOnDuplicate = false)
        {
            var existing = await _bookRepository.GetByTitleAsync(dto.Title);
            if (existing != null)
            {
                if (failOnDuplicate)
                    throw new InvalidOperationException($"A book with the title '{dto.Title}' already exists.");
                return -1;
            }

            var book = new Book
            {
                Title = dto.Title,
                Price = dto.Price
            };

            if (dto.AuthorIds.Any(a => a <= 0))
            {
                throw new ValidationException("AuthorIds must be greater than 0.");
            }
            else if (dto.AuthorIds.Count > 0)
            {
                var authors = new List<Author>();
                foreach (var authorId in dto.AuthorIds)
                {
                    var author = await _authorRepository.GetByIdAsync(authorId);
                    if (author != null)
                        authors.Add(author);
                }
                book.Authors = authors;
            }

            if (dto.GenreIds.Any(a => a <= 0))
            {
                throw new ValidationException("GenreIds must be greater than 0.");
            }
            else if (dto.GenreIds.Count > 0)
            {
                var genres = new List<Genre>();
                foreach (var genreId in dto.GenreIds)
                {
                    var genre = await _genreRepository.GetByIdAsync(genreId);
                    if (genre != null)
                        genres.Add(genre);
                }
                book.Genres = genres;
            }

            await _bookRepository.AddAsync(book);
            await _bookRepository.SaveChangesAsync();
            return book.Id;
        }

        public async Task UpdatePriceAsync(long id, decimal newPrice)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null)
                throw new KeyNotFoundException($"Book with ID {id} not found.");

            book.Price = newPrice;
            await _bookRepository.SaveChangesAsync();
        }

        public async Task DeleteBookAsync(long id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null)
                throw new KeyNotFoundException($"Book with ID {id} not found.");

            _bookRepository.Delete(book);
            await _bookRepository.SaveChangesAsync();
        }

        public async Task ImportBooksAsync(IEnumerable<BookImportDTO> importBooks)
        {
            if (importBooks == null) throw new ArgumentNullException(nameof(importBooks));

            const int batchSize = 10000;
            var stopwatch = Stopwatch.StartNew();

            var importList = importBooks as IList<BookImportDTO> ?? importBooks.ToList();
            _logger.LogInformation("Import starting. Incoming batch size: {Count}", importList.Count);

            //Preload existing data to minimize DB queries
            var existingTitles = await _bookRepository.GetAllNormalizedTitlesAsync();
            var authorsCache = await _authorRepository.GetAllNormalizedAsync();
            var genresCache = await _genreRepository.GetAllNormalizedAsync();

            int totalProcessed = 0;
            int totalSkipped = 0;
            int totalInserted = 0;

            var booksBuffer = new List<Book>(Math.Min(batchSize, Math.Max(100, importList.Count / 10)));

            try
            {
                _logger.LogInformation("Preloads complete: Titles={Titles}, Authors={Authors}, Genres={Genres}",
                    existingTitles.Count, authorsCache.Count, genresCache.Count);

                foreach (var dto in importList)
                {
                    totalProcessed++;

                    // Normalize title
                    var normalizedTitle = Book.NormalizeTitle(dto.Title);
                    if (existingTitles.Contains(normalizedTitle))
                    {
                        totalSkipped++;
                        continue; // skip duplicate
                    }

                    // Fuzzy search (with FuzzySharp for example) with a similarity threshold of 90% could be added here for more advanced duplicate detection
                    // For example in case of: 'Crime and punishment' vs. 'Criem and punishment'
                    // Fuzz.Ratio(normalizedTitle, existing) returns an integer 0-100 indicating similarity
                    // A similarity of 90 or above could be considered a duplicate so 'Crime and punishment' and 'Criem and punishment' would match
                    // For performance reasons, we can also just do a fuzzy check only against titles that are similar in length or start with the same letter

                    // Implementation example:

                    // bool isDuplicate = false;
                    // foreach (var existing in existingTitles)
                    // {
                    //     int similarity = Fuzz.Ratio(normalizedTitle, existing);
                    //     if (similarity >= 90)
                    //     {
                    //         isDuplicate = true;
                    //         _logger.LogInformation("Skipping book '{Title}' due to fuzzy match ({Similarity}%) with '{Existing}'",
                    //             dto.Title, similarity, existing);
                    //         break;
                    //     }
                    // }


                    // Build book entity
                    var book = new Book
                    {
                        Title = dto.Title?.Trim() ?? string.Empty,
                        Price = dto.Price,
                        NormalizedTitle = normalizedTitle
                    };

                    // Link authors (reuse existing or create new and add to cache)
                    foreach (var a in dto.Authors)
                    {
                        var normalizedAuthor = Author.NormalizeName(a.Name);
                        if (!authorsCache.TryGetValue(normalizedAuthor, out var authorEntity))
                        {
                            authorEntity = new Author
                            {
                                Name = a.Name?.Trim() ?? string.Empty,
                                BirthYear = a.BirthYear,
                                NormalizedName = normalizedAuthor
                            };

                            authorsCache[normalizedAuthor] = authorEntity;
                            await _authorRepository.AddAsync(authorEntity);
                        }

                        book.Authors.Add(authorEntity);
                    }

                    // Link genres (reuse existing or create new and add to cache)
                    foreach (var g in dto.Genres)
                    {
                        var normalizedGenre = Genre.NormalizeName(g.Name);
                        if (!genresCache.TryGetValue(normalizedGenre, out var genreEntity))
                        {
                            genreEntity = new Genre
                            {
                                Name = g.Name?.Trim() ?? string.Empty,
                                NormalizedName = normalizedGenre
                            };
                            genresCache[normalizedGenre] = genreEntity;
                            await _genreRepository.AddAsync(genreEntity);
                        }

                        book.Genres.Add(genreEntity);
                    }

                    existingTitles.Add(normalizedTitle);

                    booksBuffer.Add(book);

                    // When buffer full do a bulk insert
                    if (booksBuffer.Count >= batchSize)
                    {
                        await FlushBufferAsync(booksBuffer);
                        totalInserted += booksBuffer.Count;
                        booksBuffer.Clear();

                        _logger.LogInformation("Progress: processed={Processed}, inserted={Inserted}, skipped={Skipped}",
                            totalProcessed, totalInserted, totalSkipped);
                    }
                }

                // Final flush
                if (booksBuffer.Count > 0)
                {
                    await FlushBufferAsync(booksBuffer);
                    totalInserted += booksBuffer.Count;
                    booksBuffer.Clear();
                }

                stopwatch.Stop();
                _logger.LogInformation("Import finished. processed={Processed}, inserted={Inserted}, skipped={Skipped}, elapsed={Elapsed}s",
                    totalProcessed, totalInserted, totalSkipped, stopwatch.Elapsed.TotalSeconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Import failed after processing {Processed} items in {Elapsed}s", totalProcessed, stopwatch.Elapsed.TotalSeconds);
                throw;
            }
        }

        private async Task FlushBufferAsync(List<Book> buffer)
        {
            await _bookRepository.AddRangeAsync(buffer);
            await _bookRepository.SaveChangesAsync();
        }
    }
}
