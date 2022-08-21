using AmateurFootballLeague.Models;

namespace AmateurFootballLeague.ViewModels.Responses
{
    public class TeamInMatchVM
    {
        public int Id { get; set; }
        public int TeamScore { get; set; }
        public int? ScoreTieBreak { get; set; }
        public int? TeamScoreLose { get; set; }
        public int YellowCardNumber { get; set; }
        public int RedCardNumber { get; set; }
        public int ScorePenalty { get; set; }
        public int TeamInTournamentId { get; set; }
        public int MatchId { get; set; }
        public int? Result { get; set; }
        public int? WinTieBreak { get; set; }
        public string? NextTeam { get; set; }
        public string? TeamName { get; set; }
    }

    public class TeamInMatchMT
    {
        public int Id { get; set; }
        public int TeamScore { get; set; }
        public int? ScoreTieBreak { get; set; }
        public int? TeamScoreLose { get; set; }
        public int YellowCardNumber { get; set; }
        public int RedCardNumber { get; set; }
        public int? Result { get; set; }
        public int? WinTieBreak { get; set; }
        public string? NextTeam { get; set; }
        public string? TeamName { get; set; }
        public int? ScorePenalty { get; set; }
        public int TeamInTournamentId { get; set; }
        public int MatchId { get; set; }
        public virtual Match? Match { get; set; }
        public virtual TeamInTournament? TeamInTournament { get; set; }
    }

    public class TeamInMatchLV
    {
        public List<TeamInMatchVM> Teams { get; set; } = new List<TeamInMatchVM>();
        public int CurrentPage { get; set; }
        public int Size { get; set; }
    }

    public class TeamInMatchMTLV
    {
        public List<TeamInMatchMT> TeamsInMatch { get; set; } = new List<TeamInMatchMT>();
    }
}
