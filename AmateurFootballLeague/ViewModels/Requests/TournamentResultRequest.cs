namespace AmateurFootballLeague.ViewModels.Requests
{
    public enum TournamentResultFieldEnum
    {
        Id,
        Prize,
    }
    public class TournamentResultCM
    {
        public string Prize { get; set; }
        public string? Description { get; set; }
        public int TeamInTournamentId { get; set; }
        public int TournamentId { get; set; }
    }
    public class TournamentResultUM
    {
        public int Id { get; set; }
        public string? Prize { get; set; }
        public string? Description { get; set; }
        public int? TeamInTournamentId { get; set; }
        public int? TournamentId { get; set; }
    }
}
