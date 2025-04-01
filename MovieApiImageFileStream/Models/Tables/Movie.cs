namespace MovieApiImageFileStream.Models.Tables
{
	public class Movie
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public DateTime ReleaseDate { get; set; }
		public List<MovieImage> MovieImages { get; set; } = new();
	}
}
