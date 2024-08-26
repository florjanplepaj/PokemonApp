using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.DTO;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using PokemonReviewApp.Repository;

namespace PokemonReviewApp.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ReviewerController : Controller
	{
		private readonly IReviewerRepository _reviewerRepository;
		private readonly IReviewRepository _reviewRepository;
		private readonly IMapper _mapper;

		public ReviewerController(IReviewerRepository reviewerRepository,
			IReviewRepository reviewRepository,
			IMapper mapper)
		{
			_reviewerRepository = reviewerRepository;
			_reviewRepository = reviewRepository;
			_mapper = mapper;
		}

		[HttpGet]
		[ProducesResponseType(200, Type = typeof(IEnumerable<Reviewer>))]
		public IActionResult GetReviewers()
		{
			var reviewers = _mapper.Map<List<ReviewerDto>>(_reviewerRepository.GetReviewers());

			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			return Ok(reviewers);
		}

		[HttpGet("{reviewerId}")]
		[ProducesResponseType(200, Type = typeof(Reviewer))]
		[ProducesResponseType(400)]
		public IActionResult GetPokemon(int reviewerId)
		{
			if (!_reviewerRepository.ReviewerExists(reviewerId))
				return NotFound();

			var reviewer = _mapper.Map<ReviewerDto>(_reviewerRepository.GetReviewer(reviewerId));

			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			return Ok(reviewer);
		}

		[HttpGet("{reviewerId}/reviews")]
		public IActionResult GetReviewsByAReviewer(int reviewerId)
		{
			if (!_reviewerRepository.ReviewerExists(reviewerId))
				return NotFound();

			var reviews = _mapper.Map<List<ReviewDto>>(
				_reviewerRepository.GetReviewsByReviewer(reviewerId));

			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			return Ok(reviews);
		}

		[HttpPost]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		public IActionResult CreateReviewr([FromBody] ReviewerDto reviewerCreate)
		{
			if (reviewerCreate == null)
			{
				return BadRequest(ModelState);
			}
			var review = _reviewerRepository.GetReviewers()
				.Where(c => c.LastName.Trim().ToUpper() == reviewerCreate.LastName.TrimEnd().ToUpper())
				.FirstOrDefault();
			if (review != null)
			{
				ModelState.AddModelError("", "Review alraedy exists");
				return StatusCode(422, ModelState);
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);

			}
			var reviewerMapp = _mapper.Map<Reviewer>(reviewerCreate);
			
			if (!_reviewerRepository.CreateReviewer(reviewerMapp))
			{
				ModelState.AddModelError("", "Somthing went wrong while saving");
				return StatusCode(500, ModelState);
			}
			return Ok("Succesfully created");
		}
		[HttpPut("{reviewerId}")]
		[ProducesResponseType(400)]
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		public IActionResult UpdateReviewer(int reviewerId, [FromBody] ReviewerDto updatedReviewer)
		{
			if (updatedReviewer == null)
				return BadRequest(ModelState);

			if (reviewerId != updatedReviewer.Id)
				return BadRequest(ModelState);

			if (!_reviewerRepository.ReviewerExists(reviewerId))
				return NotFound();

			if (!ModelState.IsValid)
				return BadRequest();

			var reviewerMap = _mapper.Map<Reviewer>(updatedReviewer);

			if (!_reviewerRepository.UpdateReviewer(reviewerMap))
			{
				ModelState.AddModelError("", "Something went wrong updating owner");
				return StatusCode(500, ModelState);
			}

			return NoContent();
		}
		[HttpDelete("{reviewerId}")]
		[ProducesResponseType(400)]
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		public IActionResult DeleteReviewer(int reviewerId)
		{
			if (!_reviewerRepository.ReviewerExists(reviewerId))
			{
				return NotFound();
			}
			var reviewerToDelete = _reviewerRepository.GetReviewer(reviewerId);

			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			if (!_reviewerRepository.DeleteReviewer(reviewerToDelete))
			{
				ModelState.AddModelError("", "Something went wrong revieing reviewer");
			}

			return NoContent();
		}
		[HttpDelete("/DeleteReviewsByReviewer/{reviewerId}")]
		[ProducesResponseType(400)]
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		public IActionResult DeleteReviewsByReviewer(int reviewerId)
		{
			if (!_reviewerRepository.ReviewerExists(reviewerId))
				return NotFound();

			var reviewsToDelete = _reviewerRepository.GetReviewsByReviewer(reviewerId).ToList();
			if (!ModelState.IsValid)
				return BadRequest();

			if (!_reviewRepository.DeleteReviews(reviewsToDelete))
			{
				ModelState.AddModelError("", "error deleting reviews");
				return StatusCode(500, ModelState);
			}
			return NoContent();
		}


	}

}

