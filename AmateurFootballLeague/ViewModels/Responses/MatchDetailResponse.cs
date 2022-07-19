using AmateurFootballLeague.Models;

namespace AmateurFootballLeague.ViewModels.Responses
{
    public class MatchDetailVM
    {
        public int Id { get; set; }
        public int ActionMatchId { get; set; }
        public string? ActionMinute { get; set; }
        public int MatchId { get; set; }
        public int PlayerInTournamentId { get; set; }

    }

    public class MatchDetailFVM
    {
        public int Id { get; set; }
        public int ActionMatchId { get; set; }
        public string? ActionMinute { get; set; }
        public int MatchId { get; set; }
        public int PlayerInTournamentId { get; set; }
        public int? FootballPlayerId { get; set; }
        public virtual FootballPlayer? FootballPlayer { get; set; }
        public virtual Match? Match { get; set; }
        public virtual PlayerInTournament? PlayerInTournament { get; set; }
    }

    public class MatchDetailFLV
    {
        public List<MatchDetailFVM> MatchDetails { get; set; } = new List<MatchDetailFVM> ();
    }
}
