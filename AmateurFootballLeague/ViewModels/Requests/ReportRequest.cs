namespace AmateurFootballLeague.ViewModels.Requests
{
    public enum ReportFieldEnum
    {
        Id,
        Reason,
        DateReport,
        UserId,
        FootballPlayerId,
        TeamId,
        TournamentId
    }

    public enum ReportType
    {
        FootballPlayer,
        Team,
        Tournament
    }

    public class ReportCM
    {
        public string? Reason { get; set; }
        public int UserId { get; set; }
        public int? FootballPlayerId { get; set; }
        public int? TeamId { get; set; }
        public int? TournamentId { get; set; }
        public string? Status { get; set; }
    }

    public class ReportUM
    {
        public int Id { get; set; }
        public string? Reason { get; set; }
        public int? UserId { get; set; }
        public int? FootballPlayerId { get; set; }
        public int? TeamId { get; set; }
        public int? TournamentId { get; set; }
        public string? Status { get; set; }
    }
}
