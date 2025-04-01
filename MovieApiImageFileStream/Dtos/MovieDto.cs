namespace MovieApiImageFileStream.Dtos
{
	public class MovieDto
	{
		public int MovieId { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public DateTime ReleaseDate { get; set; }
		public List<MovieImageDto> MovieImages { get; set; } = new();
	}
}
