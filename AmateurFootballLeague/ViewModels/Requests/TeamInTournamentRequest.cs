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
        public int? WinScoreNumber { get; set; }
        public int? LoseScoreNumber { get; set; }
        public int? TotalYellowCard { get; set; }
        public int? TotalRedCard { get; set; }
        public string? Status { get; set; }
        public int TournamentId { get; set; }
        public int TeamId { get; set; }
    }
    public class TeamInTournamentUM
    {
        public int Id { get; set; }
        public int? Point { get; set; }
        public int? WinScoreNumber { get; set; }
        public int? LoseScoreNumber { get; set; }
        public int? TotalYellowCard { get; set; }
        public int? TotalRedCard { get; set; }
        public string? Status { get; set; }
        public string? StatusInTournament { get; set; }
        public int? TournamentId { get; set; }
        public int? TeamId { get; set; }
    }
}
