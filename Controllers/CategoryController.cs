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
	public class CategoryController : Controller
	{
		private readonly ICategoryRepository _categoryRepository;
		private readonly IMapper _mapper;

		public CategoryController(ICategoryRepository categoryRepository, IMapper mapper)
		{
			_categoryRepository = categoryRepository;
			_mapper = mapper;
		}
		[HttpGet]
		[ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
		public IActionResult GetCategories()
		{
			var categories = _mapper.Map<List<CategoryDto>>(_categoryRepository.GetCategories());



			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return Ok(categories);
		}
		[HttpGet("{categoryId}")]
		[ProducesResponseType(200, Type = typeof(Category))]
		[ProducesResponseType(400)]
		public IActionResult GetCategory(int categoryId)
		{
			if (!_categoryRepository.CategoryExists(categoryId))
			{
				return NotFound();
			}
			var category = _mapper.Map<CategoryDto>(_categoryRepository.GetCategory(categoryId));
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return Ok(category);
		}
		[HttpGet("category/{categoryId}")]
		[ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
		[ProducesResponseType(400)]
		public IActionResult GetPokemonByCategoryId(int categoryId)
		{
			var category = _mapper.Map<List<PokemonDto>>(_categoryRepository.GetPokemonByCategory(categoryId));
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return Ok(category);

		}
		[HttpPost]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		public IActionResult CreateCategory([FromBody] CategoryDto categoryCreate)
		{
			if (categoryCreate == null)
			{
				return BadRequest(ModelState);
			}
			var category = _categoryRepository.GetCategories()
				.Where(c => c.Name.Trim().ToUpper() == categoryCreate.Name.TrimEnd().ToUpper())
				.FirstOrDefault();
			if(category != null)
			{
				ModelState.AddModelError("", "Category alraedy exists");
				return StatusCode(422, ModelState);
			}
			if(!ModelState.IsValid)
			{
				return BadRequest(ModelState);

			}
			var categoryMap = _mapper.Map<Category>(categoryCreate);
			if (!_categoryRepository.CreateCategory(categoryMap))
			{
				ModelState.AddModelError("", "Somthing went wrong while saving");
				return StatusCode(500, ModelState);
			}
			return Ok("Succesfully created");
		}

		[HttpPut("{categoryId}")]
		[ProducesResponseType(400)]
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		public IActionResult UpdateCategory (int categoryId, [FromBody] CategoryDto updatecategory) {
		if(updatecategory == null)
				return BadRequest(ModelState);
		if (categoryId != updatecategory.Id)
			{ return BadRequest(ModelState); }
		if (!_categoryRepository.CategoryExists(categoryId))
				return NotFound();
			if (!ModelState.IsValid)
				return BadRequest();
		var categoryMap = _mapper.Map<Category>(updatecategory);
			if(!_categoryRepository.UpdateCategory(categoryMap))
			{
				ModelState.AddModelError("", "Somthing went wrong updating category");
					return StatusCode(500, ModelState);
			}
			return Ok("Upadate finished");


		}

		[HttpDelete("{categoryId}")]
		[ProducesResponseType(400)]
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		public IActionResult DeleteCategory(int categoryId)
		{
			if (!_categoryRepository.CategoryExists(categoryId))
			{
				return NotFound();
			}

			var categoryToDelete = _categoryRepository.GetCategory(categoryId);

			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			if (!_categoryRepository.DeleteCategory(categoryToDelete))
			{
				ModelState.AddModelError("", "Something went wrong deleting category");
			}

			return NoContent();
		}


	}
}
