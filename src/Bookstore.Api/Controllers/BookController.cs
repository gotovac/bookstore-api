using Bookstore.Core.DTOs;
using Bookstore.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quartz;

namespace Bookstore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        [Authorize(Policy = "Readers")]
        public async Task<IActionResult> GetAll()
        {
            var books = await _bookService.GetAllBooksAsync();
            return Ok(books);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "Readers")]
        public async Task<IActionResult> GetById(long id)
        {
            var book = await _bookService.GetByIdAsync(id);
            if (book == null) return NotFound();
            return Ok(book);
        }

        [HttpPost]
        [Authorize(Policy = "ReadAndWriters")]
        public async Task<IActionResult> Add([FromBody] BookCreateDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
            {
                return BadRequest("Title cannot be empty.");
            }

            var newId = await _bookService.AddBookAsync(dto, false);
            return CreatedAtAction(nameof(GetById), new { id = newId }, new { id = newId });
        }

        [HttpPut("{id}/price")]
        [Authorize(Policy = "ReadAndWriters")]
        public async Task<IActionResult> UpdatePrice(long id, [FromBody] decimal newPrice)
        {
            await _bookService.UpdatePriceAsync(id, newPrice);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "ReadAndWriters")]
        public async Task<IActionResult> Delete(long id)
        {
            await _bookService.DeleteBookAsync(id);
            return NoContent();
        }

        [HttpGet("top10")]
        [Authorize(Policy = "Readers")]
        public async Task<IActionResult> GetTop10ByRating()
        {
            var topBooks = await _bookService.GetTopRatedBooksAsync();
            return Ok(topBooks);
        }

        // For testing purposes: Manually trigger the book import job
        [HttpPost("test-import")]
        [Authorize(Policy = "ReadAndWriters")]
        public async Task<IActionResult> TestImport([FromServices] ISchedulerFactory schedulerFactory)
        {
            var scheduler = await schedulerFactory.GetScheduler();
            var jobKey = new JobKey("BookImportJob");
            await scheduler.TriggerJob(jobKey);

            return Ok("Triggered book import job manually.");
        }
    }
}
