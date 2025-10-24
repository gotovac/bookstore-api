using AutoMapper;
using Bookstore.Core.DTOs;
using Bookstore.Core.Interfaces.Repositories;
using Bookstore.Core.Interfaces.Services;
using Bookstore.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IMapper _mapper;

        public AuthorService(IAuthorRepository authorRepository, IMapper mapper)
        {
            _authorRepository = authorRepository;
            _mapper = mapper;
        }


        public async Task<IEnumerable<AuthorDTO>> GetAllAuthorsAsync()
        {
            var authors = await _authorRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<AuthorDTO>>(authors);
        }

        public async Task<AuthorDTO?> GetByIdAsync(long id)
        {
            var author = await _authorRepository.GetByIdAsync(id);
            return _mapper.Map<AuthorDTO?>(author);
        }

        public async Task<long> AddAuthorAsync(AuthorCreateDTO dto)
        {
            var existing = await _authorRepository.GetByNameAsync(dto.Name);
            if (existing != null)
                throw new InvalidOperationException("An author with the same name already exists.");

            var author = _mapper.Map<Author>(dto);
            await _authorRepository.AddAsync(author);
            await _authorRepository.SaveChangesAsync();
            return author.Id;
        }

        public async Task DeleteAuthorAsync(long id)
        {
            var author = await _authorRepository.GetByIdAsync(id);
            if (author == null)
                throw new Exception("Author not found");

            _authorRepository.Delete(author);
            await _authorRepository.SaveChangesAsync();
        }
    }
}
