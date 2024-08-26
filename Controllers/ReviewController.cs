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
	public class ReviewController : Controller
	{
		private readonly IPokemonRepository _pokemonReposityroy;
		private readonly IReviewerRepository _reviewerRepository;
		private readonly IReviewRepository _reviewRepository;
		private readonly IMapper _mapper;

		public ReviewController(IPokemonRepository pokemonReposityroy,
			IReviewerRepository reviewerRepository,
			IReviewRepository reviewRepository, IMapper mapper)
        {
			_pokemonReposityroy = pokemonReposityroy;
			_reviewerRepository = reviewerRepository;
			_reviewRepository = reviewRepository;
			_mapper = mapper;
		}

		[HttpGet]
		[ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
		public IActionResult GetReviews()
		{
			var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviews());
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return Ok(reviews);
		}


		[HttpGet("{reviewId}")]
		[ProducesResponseType(200, Type = typeof(Review))]
		[ProducesResponseType(400)]
		public IActionResult GetPokemon(int reviewId)
		{
			if (!_reviewRepository.ReviewExists(reviewId)) {
			return NotFound();
					}
			var review = _mapper.Map<ReviewDto>(_reviewRepository.GetReview(reviewId));
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return Ok(review);

		}
		[HttpGet("pokemon/{pokeId}")]
		[ProducesResponseType(200, Type = typeof(Review))]
		[ProducesResponseType(400)]
		public IActionResult GetReviewsForAPokemon(int pokeId)
		{
			var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviewsOfAPokemon(pokeId));
			if(!ModelState.IsValid) { return BadRequest(ModelState); }
			return Ok(reviews);
		}

		[HttpPost]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		public IActionResult CreateReview( [FromQuery] int reviewerId, [FromQuery] int pokeId, [FromBody] ReviewDto reviewCreate)
		{
			if (reviewCreate == null)
			{
				return BadRequest(ModelState);
			}
			var review = _reviewRepository.GetReviews()
				.Where(c => c.Title.Trim().ToUpper() == reviewCreate.Title.TrimEnd().ToUpper())
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
			var reviewMapp = _mapper.Map<Review>(reviewCreate);
			reviewMapp.Pokemon = _pokemonReposityroy.GetPokemon(pokeId);
			reviewMapp.Reviewer = _reviewerRepository.GetReviewer(reviewerId);

			if (!_reviewRepository.CreateReview(reviewMapp))
			{
				ModelState.AddModelError("", "Somthing went wrong while saving");
				return StatusCode(500, ModelState);
			}
			return Ok("Succesfully created");
		}
		[HttpPut("{reviewId}")]
		[ProducesResponseType(400)]
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		public IActionResult UpdateReview(int reviewId, [FromBody] ReviewDto updatedReview)
		{
			if (updatedReview == null)
				return BadRequest(ModelState);

			if (reviewId != updatedReview.Id)
				return BadRequest(ModelState);

			if (!_reviewRepository.ReviewExists(reviewId))
				return NotFound();

			if (!ModelState.IsValid)
				return BadRequest();

			var reviewMap = _mapper.Map<Review>(updatedReview);

			if (!_reviewRepository.UpdateReview(reviewMap))
			{
				ModelState.AddModelError("", "Something went wrong updating review");
				return StatusCode(500, ModelState);
			}

			return NoContent();
		}
		[HttpDelete("{reviewId}")]
		[ProducesResponseType(400)]
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		public IActionResult DeleteReview(int reviewId)
		{
			if (!_reviewRepository.ReviewExists(reviewId))
			{
				return NotFound();
			}
			var reviewToDelete = _reviewRepository.GetReview(reviewId);

			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			if (!_reviewRepository.DeleteReview(reviewToDelete))
			{
				ModelState.AddModelError("", "Something went wrong reviewing pokemon");
			}

			return NoContent();
		}



	}
}
