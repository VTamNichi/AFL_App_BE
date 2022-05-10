using System.ComponentModel.DataAnnotations;

namespace AmateurFootballLeague.ViewModels.Requests
{
    public class ScorePredictionCM
    {
        [Required]
        public int TeamAscore { get; set; }
        [Required]
        public int TeamBscore { get; set; }
        public string Status { get; set; }
        [Required]
        public int TeamInMatchAid { get; set; }
        [Required]
        public int TeamInMatchBid { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int MatchId { get; set; }

    }

    public class ScorePredictionUM
    {
        public int Id { get; set; }
        public int TeamAscore { get; set; }
        public int TeamBscore { get; set; }
        public string Status { get; set; }
        public int TeamInMatchAid { get; set; }
        public int TeamInMatchBid { get; set; }
        public int UserId { get; set; }
        public int MatchId { get; set; }
    }
}
