namespace AmateurFootballLeague.ViewModels.Responses
{
    public class TeamInTournamentVM
    {
        public int Id { get; set; }
        public int Point { get; set; }
        public int DifferentPoint { get; set; }
        public string Status { get; set; }
        public int TournamentId { get; set; }
        public int TeamId { get; set; }
    }
    public class TeamInTournamentListVM
    {
        public List<TeamInTournamentVM> TeamInTournaments { get; set; } = new List<TeamInTournamentVM>();
        public int CurrentPage { get; set; }
        public int Size { get; set; }
    }
}
