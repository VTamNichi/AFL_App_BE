using System.ComponentModel.DataAnnotations;

namespace AmateurFootballLeague.ViewModels.Requests
{
    public enum NewsFieldEnum
    {
        Id,
        Content
    }

    public class NewsCM
    {
        public string? Content { get; set; }
        public IFormFile? NewsImage { get; set; }

        [Required(AllowEmptyStrings = false)]
        public int TournamentId { get; set; }
    }
    public class NewsUM
    {
        [Required(AllowEmptyStrings = false)]
        public int Id { get; set; }
        public string? Content { get; set; }
        public IFormFile? NewsImage { get; set; }
        public int? TournamentId { get; set; }
    }
}
