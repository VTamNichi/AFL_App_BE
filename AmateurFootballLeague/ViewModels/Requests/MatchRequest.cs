using System.ComponentModel.DataAnnotations;

namespace AmateurFootballLeague.ViewModels.Requests
{
    public enum MatchStatusEnum
    {
        NotStart,
        Processing,
        Finished
    }
    public enum MatchFieldEnum
    {
        Id,
        MatchDate,
        Status
    }

    public class MatchCM
    {
        public DateTime MatchDate { get; set; }
        public string Status { get; set; }
        public int TournamentId { get; set; }
        public string? Round { get; set; }
        public string? Fight { get; set; }
        public string? GroupFight { get; set; }
    }

    public class MatchUM
    {
        public int Id { get; set; }
        public DateTime? MatchDate { get; set; }
        public string? Status { get; set; }
        public int? TournamentId { get; set; }
        public string? Round { get; set; }
        public string? Fight { get; set; }
        public string? GroupFight { get; set; }
        public string? CreateToken { get; set; }
    }
}
