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
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;

        public ReviewService(IReviewRepository reviewRepository, IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ReviewDTO>> GetReviewsByBookIdAsync(long bookId)
        {
            var reviews = await _reviewRepository.GetByBookIdAsync(bookId);
            return _mapper.Map<IEnumerable<ReviewDTO>>(reviews);
        }

        public async Task<double> GetAverageRatingForBookAsync(long bookId)
        {
            double rating = await _reviewRepository.GetAverageRatingForBookAsync(bookId);
            return rating;
        }

        public async Task<long> AddReviewAsync(ReviewCreateDTO dto)
        {
            var review = _mapper.Map<Review>(dto);
            await _reviewRepository.AddAsync(review);
            await _reviewRepository.SaveChangesAsync();
            return review.Id;
        }

        public async Task DeleteReviewAsync(long id)
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            if (review == null)
                throw new Exception("Review not found");

            _reviewRepository.Delete(review);
            await _reviewRepository.SaveChangesAsync();
        }
    }
}
