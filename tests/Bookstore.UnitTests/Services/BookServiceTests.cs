using AutoMapper;
using Bookstore.Core.DTOs;
using Bookstore.Core.Interfaces.Repositories;
using Bookstore.Core.Models;
using Bookstore.Services;
using Bookstore.Services.Mapping;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Bookstore.UnitTests.Services
{
    public class BookServiceTests
    {
        private readonly Mock<IBookRepository> _bookRepoMock;
        private readonly Mock<IAuthorRepository> _authorRepoMock;
        private readonly Mock<IGenreRepository> _genreRepoMock;
        private readonly Mock<ILogger<BookService>> _loggerMock;
        private readonly IMapper _mapper;
        private readonly BookService _bookService;

        public BookServiceTests()
        {
            _bookRepoMock = new Mock<IBookRepository>();
            _authorRepoMock = new Mock<IAuthorRepository>();
            _genreRepoMock = new Mock<IGenreRepository>();
            _loggerMock = new Mock<ILogger<BookService>>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            _mapper = config.CreateMapper();
            _bookService = new BookService(
                _bookRepoMock.Object,
                _authorRepoMock.Object,
                _genreRepoMock.Object,
                _mapper,
                _loggerMock.Object);
        }

        [Fact]
        public async Task AddBookAsync_ShouldSkip_WhenBookExists()
        {
            var dto = new BookCreateDTO { Title = "Existing", Price = 10m };
            _bookRepoMock.Setup(x => x.GetByTitleAsync(It.IsAny<string>()))
                         .ReturnsAsync(new Book { Title = "Existing" });

            var bookId = await _bookService.AddBookAsync(dto);

            _bookRepoMock.Verify(x => x.AddAsync(It.IsAny<Book>()), Times.Never);
        }
    }
}
