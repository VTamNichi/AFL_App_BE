using AmateurFootballLeague.Models;

namespace AmateurFootballLeague.ViewModels.Responses
{
    public class ScorePredictionVM
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

    public class ScorePredictionFVM
    {
        public int Id { get; set; }
        public int TeamAscore { get; set; }
        public int TeamBscore { get; set; }
        public string Status { get; set; }
        public int TeamInMatchAid { get; set; }
        public int TeamInMatchBid { get; set; }
        public int UserId { get; set; }
        public int MatchId { get; set; }

        public virtual Match Match { get; set; }
        public virtual TeamInMatch TeamInMatchA { get; set; }
        public virtual TeamInMatch TeamInMatchB { get; set; }
        public virtual User User { get; set; }
    }

    public class ScorePredictionLVF
    {
        public List<ScorePredictionFVM> Scores { get; set; } = new List<ScorePredictionFVM>();
        public int CurrentPage { get; set; }
        public int Size { get; set; }
    }
}
