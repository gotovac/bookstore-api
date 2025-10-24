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
    public class GenreService : IGenreService
    {
        private readonly IGenreRepository _genreRepository;
        private readonly IMapper _mapper;

        public GenreService(IGenreRepository genreRepository, IMapper mapper)
        {
            _genreRepository = genreRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GenreDTO>> GetAllGenresAsync()
        {
            var genres = await _genreRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<GenreDTO>>(genres);
        }

        public async Task<GenreDTO?> GetByIdAsync(long id)
        {
            var genre = await _genreRepository.GetByIdAsync(id);
            return _mapper.Map<GenreDTO?>(genre);
        }

        public async Task<long> AddGenreAsync(GenreCreateDTO dto)
        {
            var existing = await _genreRepository.GetByNameAsync(dto.Name);
            if (existing != null)
                throw new InvalidOperationException("A genre with the same name already exists.");

            var genre = _mapper.Map<Genre>(dto);
            await _genreRepository.AddAsync(genre);
            await _genreRepository.SaveChangesAsync();
            return genre.Id;
        }

        public async Task DeleteGenreAsync(long id)
        {
            var genre = await _genreRepository.GetByIdAsync(id);
            if (genre == null)
                throw new Exception("Genre not found");

            _genreRepository.Delete(genre);
            await _genreRepository.SaveChangesAsync();
        }
    }
}
