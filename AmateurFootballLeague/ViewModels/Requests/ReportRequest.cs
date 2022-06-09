namespace AmateurFootballLeague.ViewModels.Requests
{
    public enum ReportFieldEnum
    {
        Id,
        Reason,
        DateReport,
        UserId,
        CommentId,
        TeamId,
        TournamentId
    }

    public class ReportCM
    {
        public string? Reason { get; set; }
        public int UserId { get; set; }
        public int? CommentId { get; set; }
        public int? TeamId { get; set; }
        public int? TournamentId { get; set; }
    }

    public class ReportUM
    {
        public int Id { get; set; }
        public string? Reason { get; set; }
        public int? UserId { get; set; }
        public int? CommentId { get; set; }
        public int? TeamId { get; set; }
        public int? TournamentId { get; set; }
    }
}
