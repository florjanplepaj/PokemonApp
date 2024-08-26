using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.DTO;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using PokemonReviewApp.Repository;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace PokemonReviewApp.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PokemonController : Controller
	{
		private readonly IPokemonRepository _pokemonRepository;
		private readonly IOwnerRepository _ownerRepository;
		//private readonly ICategoryRepository _categoryRepository;
		private readonly IReviewRepository _reviewRepository;
		private readonly IMapper _mapper;

		public PokemonController(IPokemonRepository pokemonRepository,
			IOwnerRepository ownerRepository,
			//ICategoryRepository categoryRepository,
			IReviewRepository reviewRepository,
			IMapper mapper)
		{
			_pokemonRepository = pokemonRepository;
			_ownerRepository = ownerRepository;
			//_categoryRepository = categoryRepository;
			_reviewRepository = reviewRepository;
			_mapper = mapper;
		}

		[HttpGet]
		[ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
		public IActionResult GetPokemons()
		{
		var pokemons = _mapper.Map<List<PokemonDto>>(_pokemonRepository.GetPokemons());

		

		if(!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
		return Ok(pokemons);
		}

		[HttpGet("{pokeId}")]
		[ProducesResponseType(200, Type = typeof(Pokemon))]
		[ProducesResponseType(400)]
		public IActionResult GetPokemon(int pokeId)
		{
			if (!_pokemonRepository.PokemonExists(pokeId))
			{
				return NotFound();
			}
			var pokemon = _mapper.Map<PokemonDto>(_pokemonRepository.GetPokemon(pokeId));
			if(!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return Ok(pokemon);
		}
		[HttpGet("{pokeId}./rating")]
		[ProducesResponseType(200, Type = typeof(decimal))]
		[ProducesResponseType(400)]
		public IActionResult GetPokemonRating(int pokeId) { 
		if(!_pokemonRepository.PokemonExists(pokeId))
				return NotFound();
		var rating = _pokemonRepository.GetPokemonRating(pokeId);

			if (!ModelState.IsValid)
				return BadRequest();

			return Ok(rating);
		}

		[HttpPost]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		public IActionResult CreatePokemon([FromQuery] int ownerId, [FromQuery]int catId, [FromBody] PokemonDto pokemonCreate)
		{
			if (pokemonCreate == null)
			{
				return BadRequest(ModelState);
			}
			var country = _pokemonRepository.GetPokemons()
				.Where(c => c.Name.Trim().ToUpper() == pokemonCreate.Name.TrimEnd().ToUpper())
				.FirstOrDefault();
			if (country != null)
			{
				ModelState.AddModelError("", "Country alraedy exists");
				return StatusCode(422, ModelState);
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);

			}
			var pokemonMapp = _mapper.Map<Pokemon>(pokemonCreate);
			

			if (!_pokemonRepository.CreatePokemon(ownerId,catId,pokemonMapp))
			{
				ModelState.AddModelError("", "Somthing went wrong while saving");
				return StatusCode(500, ModelState);
			}
			return Ok("Succesfully created");
		}

		[HttpPut("{pokeId}")]
		[ProducesResponseType(400)]
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		public IActionResult UpdatePokemon( int pokeId,[FromQuery] int ownerId, [FromQuery] int catId,
			[FromBody] PokemonDto updatepokemon)
		{
			if (updatepokemon == null)
				return BadRequest(ModelState);
			if (pokeId != updatepokemon.Id)
			{ return BadRequest(ModelState); }
			if (!_pokemonRepository.PokemonExists(pokeId))
				return NotFound();
			if (!ModelState.IsValid)
				return BadRequest();
			var pokemonMap = _mapper.Map<Pokemon>(updatepokemon);
			if (!_pokemonRepository.UpdatePokemon(ownerId, catId,pokemonMap))
			{
				ModelState.AddModelError("", "Somthing went wrong updating country");
				return StatusCode(500, ModelState);
			}
			return Ok("Upadate finished");


		}

		[HttpDelete("{pokemonId}")]
		[ProducesResponseType(400)]
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		public IActionResult DeletePokemon(int pokemonId)
		{
			if (!_pokemonRepository.PokemonExists(pokemonId))
			{
				return NotFound();
			}
			var reviewsToDelete = _reviewRepository.GetReviewsOfAPokemon(pokemonId);
			var pokemonToDelete = _pokemonRepository.GetPokemon(pokemonId);

			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			if(!_reviewRepository.DeleteReviews(reviewsToDelete.ToList()))
			{
				ModelState.AddModelError("", "Something went wrong deleting reviews");
			}

			if (!_pokemonRepository.DeletePokemon(pokemonToDelete))
			{
				ModelState.AddModelError("", "Something went wrong deleting pokemon");
			}

			return NoContent();
		}
	}
}
