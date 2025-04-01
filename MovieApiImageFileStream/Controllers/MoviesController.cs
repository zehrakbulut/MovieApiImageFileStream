using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieApiImageFileStream.Dtos;
using MovieApiImageFileStream.Models.Tables;

namespace MovieApiImageFileStream.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class MoviesController : ControllerBase
	{
		private readonly AppDbContext _context;

		public MoviesController(AppDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<MovieDto>>> GetMovies()
		{
			var movies = await _context.Movies.Include(m => m.MovieImages).ToListAsync();

			var movieDtos = movies.Select(m => new MovieDto
			{
				MovieId = m.Id,
				Title = m.Title,
				Description = m.Description,
				ReleaseDate = m.ReleaseDate,
				MovieImages = m.MovieImages.Select(img => new MovieImageDto
				{
					MovieImageId = img.Id,
					FilePath = img.FilePath,
					IsCover = img.IsCover
				}).ToList()
			}).ToList();

			return Ok(movieDtos);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<MovieDto>> GetMovie(int id)
		{
			var movie = await _context.Movies.Include(m => m.MovieImages)
											 .FirstOrDefaultAsync(m => m.Id == id);

			if (movie == null)
			{
				return NotFound();
			}

			var movieDto = new MovieDto
			{
				MovieId = movie.Id,
				Title = movie.Title,
				Description = movie.Description,
				ReleaseDate = movie.ReleaseDate,
				MovieImages = movie.MovieImages.Select(img => new MovieImageDto
				{
					MovieImageId = img.Id,
					FilePath = img.FilePath,
					IsCover = img.IsCover
				}).ToList()
			};

			return Ok(movieDto);
		}

		[HttpPost("add")]
		public async Task<ActionResult<MovieDto>> AddMovie(MovieDto movieDto)
		{
			var movie = new Movie
			{
				Title = movieDto.Title,
				Description = movieDto.Description,
				ReleaseDate = movieDto.ReleaseDate,
				MovieImages = new List<MovieImage>() 
			};

			_context.Movies.Add(movie);
			await _context.SaveChangesAsync();

			var createdMovie = new MovieDto
			{
				MovieId = movie.Id, 
				Title = movie.Title,
				Description = movie.Description,
				ReleaseDate = movie.ReleaseDate,
				MovieImages = new List<MovieImageDto>() 
			};

			return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, createdMovie);
		}



		[HttpPut("update/{id}")]
		public async Task<IActionResult> UpdateMovie(int id, MovieDto movieDto)
		{
			var movie = await _context.Movies.Include(m => m.MovieImages)
											 .FirstOrDefaultAsync(m => m.Id == id);

			if (movie == null)
			{
				return NotFound();
			}

			movie.Title = movieDto.Title;
			movie.Description = movieDto.Description;
			movie.ReleaseDate = movieDto.ReleaseDate;

			movie.MovieImages = movieDto.MovieImages.Select(img => new MovieImage
			{
				FilePath = img.FilePath,
				IsCover = img.IsCover
			}).ToList();

			await _context.SaveChangesAsync();

			return NoContent();
		}

		[HttpDelete("delete/{id}")]
		public async Task<IActionResult> DeleteMovie(int id)
		{
			var movie = await _context.Movies.FindAsync(id);
			if (movie == null)
			{
				return NotFound();
			}

			_context.Movies.Remove(movie);
			await _context.SaveChangesAsync();

			return NoContent();
		}
	}
}
