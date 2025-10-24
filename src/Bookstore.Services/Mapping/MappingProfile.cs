using AutoMapper;
using Bookstore.Core.DTOs;
using Bookstore.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Services.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {

            // Book <-> DTOs
            CreateMap<Book, BookDTO>()
                .ForMember(dest => dest.Authors, opt => opt.MapFrom(src => src.Authors.Select(a => a.Name)))
                .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.Genres.Select(g => g.Name)))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src =>
                    src.AverageRating != 0 ? src.AverageRating : (src.Reviews.Count != 0 ? src.Reviews.Average(r => r.Rating) : 0)));


            CreateMap<BookCreateDTO, Book>();
            CreateMap<PriceUpdateDTO, Book>();

            // Author
            CreateMap<Author, AuthorDTO>().ReverseMap();
            CreateMap<AuthorCreateDTO, Author>();
            CreateMap<AuthorUpdateDTO, Author>();

            // Genre
            CreateMap<Genre, GenreDTO>().ReverseMap();
            CreateMap<GenreCreateDTO, Genre>();
            CreateMap<GenreUpdateDTO, Genre>();

            // Review
            CreateMap<Review, ReviewDTO>().ReverseMap();
            CreateMap<ReviewCreateDTO, Review>();

            // Import
            CreateMap<BookImportDTO, Book>()
                .ForMember(dest => dest.Authors, opt => opt.MapFrom(src => src.Authors.Select(a => new Author { Name = a.Name, BirthYear = a.BirthYear })))
                .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.Genres.Select(g => new Genre { Name = g.Name })));
        }
    }
}
