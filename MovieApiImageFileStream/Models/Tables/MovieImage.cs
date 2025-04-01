using System.ComponentModel.DataAnnotations.Schema;

namespace MovieApiImageFileStream.Models.Tables
{
	public class MovieImage
	{
		public int Id { get; set; }
		public string FilePath { get; set; } = string.Empty; // Dosya yolu
		public bool IsCover { get; set; }
		public int MovieId { get; set; }
		public Movie Movie { get; set; }
	}
}
