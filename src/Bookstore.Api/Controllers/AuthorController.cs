using Bookstore.Core.DTOs;
using Bookstore.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        public AuthorController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        [HttpGet]
        [Authorize(Policy = "Readers")]
        public async Task<IActionResult> GetAll()
        {
            var authors = await _authorService.GetAllAuthorsAsync();
            return Ok(authors);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "Readers")]
        public async Task<IActionResult> GetById(long id)
        {
            var author = await _authorService.GetByIdAsync(id);
            if (author == null) return NotFound();
            return Ok(author);
        }

        [HttpPost]
        [Authorize(Policy = "ReadAndWriters")]
        public async Task<IActionResult> Add([FromBody] AuthorCreateDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest("Name cannot be empty.");
            }

            var newId = await _authorService.AddAuthorAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = newId }, new { id = newId });
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "ReadAndWriters")]
        public async Task<IActionResult> Delete(long id)
        {
            await _authorService.DeleteAuthorAsync(id);
            return NoContent();
        }
    }
}
