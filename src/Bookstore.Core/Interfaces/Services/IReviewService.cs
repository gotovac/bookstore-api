using Bookstore.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Core.Interfaces.Services
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDTO>> GetReviewsByBookIdAsync(long bookId);
        Task<double> GetAverageRatingForBookAsync(long bookId);
        Task<long> AddReviewAsync(ReviewCreateDTO dto);
        Task DeleteReviewAsync(long id);
    }
}
