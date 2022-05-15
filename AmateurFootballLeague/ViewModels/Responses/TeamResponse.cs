using AmateurFootballLeague.Models;

namespace AmateurFootballLeague.ViewModels.Responses
{
    public class TeamVM
    {
        public int Id { get; set; }
        public string TeamName { get; set; }
        public string TeamAvatar { get; set; }
        public string TeamArea { get; set; }
        public string TeamPhone { get; set; }
        public string TeamGender { get; set; }
        public DateTime DateCreate { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public int NumberPlayerInTeam { get; set; }
    }
    public class TeamFVM
    {
        public int Id { get; set; }
        public string? TeamName { get; set; }
        public string? TeamAvatar { get; set; }
        public string? Description { get; set; }
        public bool? Status { get; set; }
        public DateTime? DateCreate { get; set; }
        public DateTime? DateUpdate { get; set; }
        public DateTime? DateDelete { get; set; }

        public virtual User IdNavigation { get; set; } = null!;
        public virtual ICollection<PlayerInTeam> PlayerInTeams { get; set; }
        public virtual ICollection<TeamInMatch> TeamInMatches { get; set; }
        public virtual ICollection<TeamInTournament> TeamInTournaments { get; set; }
    }
    public class TeamListVM
    {
        public List<TeamVM> Teams { get; set; } = new List<TeamVM>();
        public int CountList { get; set; }
        public int CurrentPage { get; set; }
        public int Size { get; set; }
    }
}
