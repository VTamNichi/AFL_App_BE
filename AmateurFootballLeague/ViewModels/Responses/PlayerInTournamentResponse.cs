namespace AmateurFootballLeague.ViewModels.Responses
{
    public class PlayerInTournamentVM
    {
        public int Id { get; set; }
        public int TeamInTournamentId { get; set; }
        public int PlayerInTeamId { get; set; }
    }
    public class PlayerInTournamentLV
    {
        public List<PlayerInTournamentVM> PlayerInTournaments { get; set; } = new List<PlayerInTournamentVM>(); 
        public int CurrentPage { get; set; }
        public int Size { get; set; }
    }
}
