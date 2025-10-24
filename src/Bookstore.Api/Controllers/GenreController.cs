using Bookstore.Core.DTOs;
using Bookstore.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenreController : ControllerBase
    {
        private readonly IGenreService _genreService;

        public GenreController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        [HttpGet]
        [Authorize(Policy = "Readers")]
        public async Task<IActionResult> GetAll()
        {
            var genres = await _genreService.GetAllGenresAsync();
            return Ok(genres);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "Readers")]
        public async Task<IActionResult> GetById(long id)
        {
            var genre = await _genreService.GetByIdAsync(id);
            if (genre == null) return NotFound();
            return Ok(genre);
        }

        [HttpPost]
        [Authorize(Policy = "ReadAndWriters")]
        public async Task<IActionResult> Add([FromBody] GenreCreateDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest("Name cannot be empty.");
            }

            var newId = await _genreService.AddGenreAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = newId }, new { id = newId });
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "ReadAndWriters")]
        public async Task<IActionResult> Delete(long id)
        {
            await _genreService.DeleteGenreAsync(id);
            return NoContent();
        }
    }
}
