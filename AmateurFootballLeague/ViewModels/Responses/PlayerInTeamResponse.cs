using AmateurFootballLeague.Models;

namespace AmateurFootballLeague.ViewModels.Responses
{
    public class PlayerInTeamVM
    {
        public int Id { get; set; }
        public string? Status { get; set; }
        public int TeamId { get; set; }
        public int FootballPlayerId { get; set; }
    }

    public class PlayerInTeamFVM
    {
        public int Id { get; set; }
        public string? Status { get; set; }
        public int TeamId { get; set; }
        public int FootballPlayerId { get; set; }
        public virtual FootballPlayer? FootballPlayer { get; set; }
        public virtual Team? Team { get; set; }
    }

    public class PlayerInTeamLV
    {
        public List<PlayerInTeamVM> PlayerInTeams =  new();
        public int CountList { get; set; }
        public int CurrentPage { get; set; }
        public int Size { get; set; }
    }

    public class PlayerInTeamLFV
    {
        public List<PlayerInTeamFVM> PlayerInTeamsFull = new();
        public int CountList { get; set; }
        public int CurrentPage { get; set; }
        public int Size { get; set; }
    }
}
