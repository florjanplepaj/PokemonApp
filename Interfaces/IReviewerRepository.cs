﻿using PokemonReviewApp.Models;

namespace PokemonReviewApp.Interfaces
{
	public interface IReviewerRepository
	{
		ICollection<Reviewer> GetReviewers();
		Reviewer GetReviewer(int id);
		ICollection<Review> GetReviewsByReviewer(int reviewerId);
		bool ReviewerExists(int reviewerid);
		bool CreateReviewer(Reviewer reviewer);
		bool UpdateReviewer(Reviewer reviewer);
		bool DeleteReviewer(Reviewer reviewer);
		bool Save();
	}
}
