namespace AmateurFootballLeague.ViewModels.Requests
{
    public enum TeamInTournamentFieldEnum
    {
        Id,
        Point,
        DifferentPoint
    }
    public class TeamInTournamentCM
    {
        public int? Point { get; set; }
        public int? DifferentPoint { get; set; }
        public string? Status { get; set; }
        public int TournamentId { get; set; }
        public int TeamId { get; set; }
    }
    public class TeamInTournamentUM
    {
        public int Id { get; set; }
        public int? Point { get; set; }
        public int? DifferentPoint { get; set; }
        public string? Status { get; set; }
        public int? TournamentId { get; set; }
        public int? TeamId { get; set; }
    }
}
