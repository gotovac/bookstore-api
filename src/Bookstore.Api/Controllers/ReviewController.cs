using Bookstore.Core.DTOs;
using Bookstore.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("book/{bookId}")]
        [Authorize(Policy = "Readers")]
        public async Task<IActionResult> GetByBook(long bookId)
        {
            var reviews = await _reviewService.GetReviewsByBookIdAsync(bookId);
            return Ok(reviews);
        }

        [HttpGet("book/{bookId}/average")]
        [Authorize(Policy = "Readers")]
        public async Task<IActionResult> GetAverageRating(long bookId)
        {
            var avg = await _reviewService.GetAverageRatingForBookAsync(bookId);
            return Ok(avg);
        }

        [HttpPost]
        [Authorize(Policy = "ReadAndWriters")]
        public async Task<IActionResult> Add([FromBody] ReviewCreateDTO dto)
        {
            await _reviewService.AddReviewAsync(dto);
            return CreatedAtAction(nameof(GetByBook), new { bookId = dto.BookId }, new { bookId = dto.BookId });
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "ReadAndWriters")]
        public async Task<IActionResult> Delete(long id)
        {
            await _reviewService.DeleteReviewAsync(id);
            return NoContent();
        }
    }
}
