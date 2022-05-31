using System.ComponentModel.DataAnnotations;

namespace AmateurFootballLeague.ViewModels.Requests
{
    public class CommentCM
    {
        [Required]
        public string Content { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime? DateDelete { get; set; }
        public DateTime? DateUpdate { get; set; }
        public string Status { get; set; }
        public int? TeamId { get; set; }
        public int? TournamentId { get; set; }
        [Required]
        public int UserId { get; set; }
    }

    public class CommentUM
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Content { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime? DateDelete { get; set; }
        public DateTime? DateUpdate { get; set; }
        public string Status { get; set; }
        public int? TeamId { get; set; }
        public int? TournamentId { get; set; }
        [Required]
        public int UserId { get; set; }
    }
}
