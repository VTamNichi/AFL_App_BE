using System.ComponentModel.DataAnnotations;

namespace AmateurFootballLeague.ViewModels.Requests
{
    public class TeamInMatchCM
    {
        [Required]
        public int TeamScore { get; set; }
        public int? ScoreTieBreak { get; set; }
        public int? TeamScoreLose { get; set; }
        public int? YellowCardNumber { get; set; }
        public int? RedCardNumber { get; set; }
        [Required]
        public int TeamInTournamentId { get; set; }
        [Required]
        public int MatchId { get; set; }
        public int? Result { get; set; }
        public int? WinTieBreak { get; set; }
        public string? NextTeam { get; set; }
        public string? TeamName { get; set; }
    }

    public class TeamInMatchUM
    {
        public int Id { get; set; }
        public int? TeamScore { get; set; }
        public int? ScoreTieBreak { get; set; }
        public int? TeamScoreLose { get; set; }
        public int? YellowCardNumber { get; set; }
        public int? RedCardNumber { get; set; }
        public int? ScorePenalty { get; set; }
        public int TeamInTournamentId { get; set; }
        public int MatchId { get; set; }
        public int? Result { get; set; }
        public int? WinTieBreak { get; set; }
        public string? NextTeam { get; set; }
        public string? TeamName { get; set; }
    }

    public class TeamInMatchToTournamentUM
    {
        public int TeamInTournamentId { get; set; }
        public bool TypeUpdate { get; set; }
        public int? TeamIndex { get; set; }
    }

    public enum GroupName
    {
        A,
        B,
        C,
        D
    }
    public class TeamInMatchNextUM
    {
        public int TournamentId { get; set; }
        public int MatchId { get; set; }
        public GroupName? GroupName { get; set; }

    }
}
