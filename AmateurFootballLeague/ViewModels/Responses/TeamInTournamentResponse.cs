using AmateurFootballLeague.Models;

namespace AmateurFootballLeague.ViewModels.Responses
{
    public class TeamInTournamentVM
    {
        public int Id { get; set; }
        public int Point { get; set; }
        public int? WinScoreNumber { get; set; }
        public int? LoseScoreNumber { get; set; }
        public int? DifferentPoint { get; set; }
        public int? TotalYellowCard { get; set; }
        public int? TotalRedCard { get; set; }
        public int? WinTieBreak { get; set; }
        public string? GroupName { get; set; }
        public string? Status { get; set; }
        public string? StatusInTournament { get; set; }
        public int TournamentId { get; set; }
        public int TeamId { get; set; }
        public Team? Team { get; set; }
        public int numberOfMatch { get; set; }
        public int numberOfWin { get; set; }
        public int numberOfLose { get; set; }
        public int numberOfDraw { get; set; }
    }
    public class TeamInTournamentListVM
    {
        public List<TeamInTournamentVM> TeamInTournaments { get; set; } = new List<TeamInTournamentVM>();
        public int CountList { get; set; }
        public int CurrentPage { get; set; }
        public int Size { get; set; }
    }
}
