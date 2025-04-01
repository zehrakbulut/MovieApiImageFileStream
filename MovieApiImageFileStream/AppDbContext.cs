using Microsoft.EntityFrameworkCore;
using MovieApiImageFileStream.Models.Tables;

namespace MovieApiImageFileStream
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }	

		public DbSet<Movie> Movies { get; set; }	
		public DbSet<MovieImage> MoviesImages { get; set; }
	}
}
