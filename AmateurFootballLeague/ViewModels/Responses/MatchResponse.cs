using AmateurFootballLeague.Models;

namespace AmateurFootballLeague.ViewModels.Responses
{
    public class MatchVM
    {
        public int Id { get; set; }
        public string MatchDate { get; set; }
        public string Status { get; set; }
        public int TournamentId { get; set; }
        public string Round { get; set; }
        public string Fight { get; set; }
        public string GroupFight { get; set; }
        public string TokenLivestream { get; set; }
    }

    public class MatchFVM
    {
        public int Id { get; set; }
        public string MatchDate { get; set; }
        public string Status { get; set; }
        public int TournamentId { get; set; }
        public string Round { get; set; }
        public string Fight { get; set; }
        public string GroupFight { get; set; }
        public string TokenLivestream { get; set; }
        public virtual Tournament Tournament { get; set; }
        public virtual ICollection<MatchDetail> MatchDetails { get; set; }
        public virtual ICollection<ScorePrediction> ScorePredictions { get; set; }
        public virtual ICollection<TeamInMatch> TeamInMatches { get; set; }
    }
    public class MatchListVM
    {
        public List<MatchVM> Matchs { get; set; } = new List<MatchVM>();
        public int CountList { get; set; }
        public int CurrentPage { get; set; }
        public int Size { get; set; }
    }

    public class MatchListFVM
    {
        public List<MatchFVM> Matchs { get; set; } = new List<MatchFVM>();
        public int CurrentPage { get; set; }
        public int Size { get; set; }
    }
}
