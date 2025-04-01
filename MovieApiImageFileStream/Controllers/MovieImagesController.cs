﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieApiImageFileStream.Models.Tables;

namespace MovieApiImageFileStream.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class MovieImagesController : ControllerBase
	{
		private readonly AppDbContext _context;
		private readonly IWebHostEnvironment _environment;

		public MovieImagesController(AppDbContext context, IWebHostEnvironment environment)
		{
			_context = context;
			_environment = environment;
		}

		// 📌 1️⃣ Resim Yükleme
		[HttpPost("{movieId}/upload")]
		public async Task<IActionResult> UploadImage(int movieId, IFormFile file, bool isCover = false)
		{
			if (file == null || file.Length == 0)
				return BadRequest(new { message = "Dosya seçilmedi." });

			var allowedTypes = new List<string> { "image/jpeg", "image/png" };
			if (!allowedTypes.Contains(file.ContentType))
				return BadRequest(new { message = "Geçersiz dosya türü." });

			var movie = await _context.Movies.FindAsync(movieId);
			if (movie == null)
				return NotFound(new { message = "Film bulunamadı." });

			var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
			if (!Directory.Exists(uploadsFolder))
				Directory.CreateDirectory(uploadsFolder);

			var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
			var filePath = Path.Combine(uploadsFolder, fileName);

			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}

			var movieImage = new MovieImage
			{
				FilePath = $"/uploads/{fileName}",
				IsCover = isCover,
				MovieId = movieId
			};

			_context.MoviesImages.Add(movieImage);
			await _context.SaveChangesAsync();

			return Ok(new { message = "Resim yüklendi!", filePath = movieImage.FilePath });
		}

		// 📌 2️⃣ Tüm Resimleri Listeleme
		[HttpGet("all")]
		public async Task<ActionResult<IEnumerable<MovieImage>>> GetAllImages()
		{
			return await _context.MoviesImages.ToListAsync();
		}

		// 📌 3️⃣ Belirli Bir Filmin Tüm Resimlerini Listeleme
		[HttpGet("movie/{movieId}")]
		public async Task<ActionResult<IEnumerable<MovieImage>>> GetImagesByMovie(int movieId)
		{
			var images = await _context.MoviesImages.Where(img => img.MovieId == movieId).ToListAsync();
			return images.Any() ? Ok(images) : NotFound(new { message = "Bu filme ait resim bulunamadı." });
		}

		// 📌 4️⃣ Tek Bir Resmi Getirme
		[HttpGet("get/{imageId}")]
		public async Task<IActionResult> GetImage(int imageId)
		{
			var movieImage = await _context.MoviesImages.FindAsync(imageId);
			if (movieImage == null)
				return NotFound(new { message = "Resim bulunamadı." });

			var filePath = Path.Combine(_environment.WebRootPath, movieImage.FilePath.TrimStart('/'));
			if (!System.IO.File.Exists(filePath))
				return NotFound(new { message = "Dosya bulunamadı." });

			var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
			return File(fileBytes, "image/jpeg"); // PNG de olabilir
		}

		// 📌 5️⃣ Resim Güncelleme (Yeniden Yükleme)
		[HttpPut("update/{imageId}")]
		public async Task<IActionResult> UpdateImage(int imageId, IFormFile file, bool isCover = false)
		{
			var movieImage = await _context.MoviesImages.FindAsync(imageId);
			if (movieImage == null)
				return NotFound(new { message = "Resim bulunamadı." });

			if (file == null || file.Length == 0)
				return BadRequest(new { message = "Dosya seçilmedi." });

			var allowedTypes = new List<string> { "image/jpeg", "image/png" };
			if (!allowedTypes.Contains(file.ContentType))
				return BadRequest(new { message = "Geçersiz dosya türü." });

			var oldFilePath = Path.Combine(_environment.WebRootPath, movieImage.FilePath.TrimStart('/'));
			if (System.IO.File.Exists(oldFilePath))
				System.IO.File.Delete(oldFilePath);

			var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
			var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
			var filePath = Path.Combine(uploadsFolder, fileName);

			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}

			movieImage.FilePath = $"/uploads/{fileName}";
			movieImage.IsCover = isCover;

			await _context.SaveChangesAsync();

			return Ok(new { message = "Resim güncellendi!", filePath = movieImage.FilePath });
		}

		// 📌 6️⃣ Resim Silme
		[HttpDelete("delete/{imageId}")]
		public async Task<IActionResult> DeleteImage(int imageId)
		{
			var movieImage = await _context.MoviesImages.FindAsync(imageId);
			if (movieImage == null)
				return NotFound(new { message = "Resim bulunamadı." });

			var filePath = Path.Combine(_environment.WebRootPath, movieImage.FilePath.TrimStart('/'));
			if (System.IO.File.Exists(filePath))
				System.IO.File.Delete(filePath);

			_context.MoviesImages.Remove(movieImage);
			await _context.SaveChangesAsync();

			return Ok(new { message = "Resim başarıyla silindi." });
		}
	}
}