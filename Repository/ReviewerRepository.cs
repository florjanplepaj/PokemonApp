using AutoMapper;
using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
	public class ReviewerRepository : IReviewerRepository
	{
		private readonly DataContext _context;
		private readonly IMapper _mapper;

		public ReviewerRepository(DataContext context, IMapper mapper)
        {
			_context = context;
			_mapper = mapper;
		}

		public bool CreateReviewer(Reviewer reviewer)
		{
			_context.Add(reviewer);
			return Save();
		}

		public bool DeleteReviewer(Reviewer reviewer)
		{
			_context.Remove(reviewer);
			return Save();
		}

		public Reviewer GetReviewer(int id)
		{
			return _context.Reviewers.Where(a => a.Id == id).FirstOrDefault();
		}

		public ICollection<Reviewer> GetReviewers()
		{
			return _context.Reviewers.ToList();	
		}

		public ICollection<Review> GetReviewsByReviewer(int reviewerId)
		{
			return _context.Reviews.Where(a => a.Reviewer.Id == reviewerId).ToList();
		}

		public bool ReviewerExists(int reviewerid)
		{
			if(_context.Reviewers.Any(a => a.Id == reviewerid)) return true;
				return false;
		}

		public bool Save()
		{
			var saved = _context.SaveChanges();
			return saved > 0 ? true : false;
		}

		public bool UpdateReviewer(Reviewer reviewer)
		{
			_context.Update(reviewer);
			return Save();	
		}
	}
}
