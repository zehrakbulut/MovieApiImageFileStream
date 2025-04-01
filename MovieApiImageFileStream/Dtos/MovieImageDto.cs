namespace MovieApiImageFileStream.Dtos
{
	public class MovieImageDto
	{
		public int MovieImageId { get; set; }
		public string FilePath { get; set; } = string.Empty;
		public bool IsCover { get; set; }
	}
}
