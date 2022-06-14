using System.ComponentModel.DataAnnotations;

namespace AmateurFootballLeague.ViewModels.Requests
{
    public class ImageCM
    {
        [Required(AllowEmptyStrings = false)]
        public IFormFile File { get; set; }

        [Required(AllowEmptyStrings = false)]
        public int TournamentId { get; set; }
    }
    public class ImageUM
    {
        [Required(AllowEmptyStrings = false)]
        public int Id { get; set; }
        public IFormFile? File { get; set; }
        public int? TournamentId { get; set; }
    }

    public class TestImage
    {
        public IFormFile File { get; set; }
    }
}
