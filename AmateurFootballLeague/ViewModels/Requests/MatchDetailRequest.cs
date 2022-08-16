using System.ComponentModel.DataAnnotations;

namespace AmateurFootballLeague.ViewModels.Requests
{
    public enum DeleteType
    {
        score,
        yellow,
        red,
        penalty
    }
    public class MatchDetailCM
    {
        public int ActionMatchId { get; set; }
        public string? ActionMinute { get; set; }
        [Required]
        public int MatchId { get; set; }
        [Required]
        public int PlayerInTournamentId { get; set; }
        [Required]
        public int FootballPlayerId { get; set; }
        public bool? StatusPen { get; set; }
    }

    public class MatchDetailUM
    {
        public int Id { get; set; }
        public int ActionMatchId { get; set; }
        public string? ActionMinute { get; set; }
        public int MatchId { get; set; }
        public int PlayerInTournamentId { get; set; }
        public bool? StatusPen { get; set; }
    }

}
