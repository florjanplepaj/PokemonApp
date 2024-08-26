using AutoMapper;
using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
	public class ReviewRepository : IReviewRepository
	{
		private readonly DataContext _context;
		private readonly IMapper _mapper;

		public ReviewRepository(DataContext context,IMapper mapper)
        {
			_context = context;
			_mapper = mapper;
		}
        public ICollection<Review> GetReviews()
		{
			return _context.Reviews.ToList();
		}

		public Review GetReview(int reviewid)
		{
			return _context.Reviews.Where(a => a.Id == reviewid).FirstOrDefault();

		}

		public ICollection<Review> GetReviewsOfAPokemon(int pokeId)
		{
			return _context.Reviews.Where(a => a.Pokemon.Id == pokeId).ToList();
		}

		public bool ReviewExists(int reviewId)
		{
			if( _context.Reviews.Any(a => a.Id == reviewId)) return true;
			return false;
		}

		public bool CreateReview(Review review)
		{
			_context.Add(review);
			return Save();
		}

		public bool Save()
		{
			var saved = _context.SaveChanges();
			return saved > 0 ? true : false;
		}

		public bool UpdateReview(Review review)
		{
			_context.Update(review);
			return Save();
		}

		public bool DeleteReview(Review review)
		{
			_context.Remove(review);
			return Save();
		}

		public bool DeleteReviews(List<Review> reviews)
		{
			_context.RemoveRange(reviews);
			return Save();
		}
	}
}
