using AmateurFootballLeague.Models;

namespace AmateurFootballLeague.ViewModels.Responses
{
    public class ReportVM
    {
        public int Id { get; set; }
        public string? Reason { get; set; }
        public string? DateReport { get; set; }
        public int UserId { get; set; }
        public int FootballPlayerId { get; set; }
        public int TeamId { get; set; }
        public int TournamentId { get; set; }
        public User? User { get; set; }
    }

    public class ReportListVM
    {
        public List<ReportVM> Reports { get; set; } = new List<ReportVM>();
        public int CountList { get; set; }
        public int CurrentPage { get; set; }
        public int Size { get; set; }
    }
}
