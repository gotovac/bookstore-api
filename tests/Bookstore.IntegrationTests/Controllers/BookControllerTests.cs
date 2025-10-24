using AutoMapper;
using Bookstore.Api.Controllers;
using Bookstore.Core.DTOs;
using Bookstore.Core.Models;
using Bookstore.Data;
using Bookstore.Data.Repositories;
using Bookstore.Services;
using Bookstore.Services.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Bookstore.IntegrationTests.Controllers
{
    public class BookControllerTests
    {
        private readonly AppDbContext _context;
        private readonly BookService _bookService;
        private readonly BookController _bookController;
        private readonly IMapper _mapper;

        public BookControllerTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "BookControllerTestDb")
                .Options;

            _context = new AppDbContext(options);

            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();

            var logger = new LoggerFactory().CreateLogger<BookService>();

            _bookService = new BookService(
                new BookRepository(_context),
                new AuthorRepository(_context),
                new GenreRepository(_context),
                _mapper,
                logger);

            _bookController = new BookController(_bookService);
        }

        [Fact]
        public async Task AddBook_ReturnsBookId()
        {
            var author = new Author { Name = "Author 2", BirthYear = 1980, NormalizedName = "author 2" };
            var genre = new Genre { Name = "Genre 2", NormalizedName = "genre 2" };
            _context.Authors.Add(author);
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            var dto = new BookCreateDTO
            {
                Title = "New Book",
                Price = 12.5m,
                AuthorIds = new List<long> { author.Id },
                GenreIds = new List<long> { genre.Id }
            };

            var result = await _bookController.Add(dto) as CreatedAtActionResult;
            Assert.NotNull(result);

            var value = result.Value;
            Assert.NotNull(value);

            var idProperty = value.GetType().GetProperty("id");
            Assert.NotNull(idProperty);

            var idValue = (long)idProperty.GetValue(value);
            Assert.True(idValue > 0);
        }
    }
}
