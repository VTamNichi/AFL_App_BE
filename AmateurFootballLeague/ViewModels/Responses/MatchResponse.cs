using AmateurFootballLeague.Models;

namespace AmateurFootballLeague.ViewModels.Responses
{
    public class MatchVM
    {
        public int Id { get; set; }
        public DateTime MatchDate { get; set; }
        public string Status { get; set; }
    }

    public class MatchFVM
    {
        public int Id { get; set; }
        public DateTime MatchDate { get; set; }
        public string Status { get; set; }
        public int TournamentId { get; set; }

        public virtual Tournament Tournament { get; set; }
        public virtual ICollection<MatchDetail> MatchDetails { get; set; }
        public virtual ICollection<ScorePrediction> ScorePredictions { get; set; }
        public virtual ICollection<TeamInMatch> TeamInMatches { get; set; }
    }
    public class MatchListVM
    {
        public List<MatchVM> Matchs { get; set; } = new List<MatchVM>();
        public int CurrentPage { get; set; }
        public int Size { get; set; }
    }
}
