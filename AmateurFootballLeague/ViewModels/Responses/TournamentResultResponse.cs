namespace AmateurFootballLeague.ViewModels.Responses
{
    public class TournamentResultVM
    {
        public int Id { get; set; }
        public string? Prize { get; set; }
        public string? Description { get; set; }
        public int TeamInTournamentId { get; set; }
        public int TournamentId { get; set; }
    }
    public class TournamentResultListVM
    {
        public List<TournamentResultVM> TournamentResults { get; set; } = new List<TournamentResultVM>();
        public int CurrentPage { get; set; }
        public int Size { get; set; }
    }
}
