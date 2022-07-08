using AmateurFootballLeague.Models;

namespace AmateurFootballLeague.ViewModels.Responses
{
    public class PlayerInTournamentVM
    {
        public int Id { get; set; }
        public int TeamInTournamentId { get; set; }
        public int PlayerInTeamId { get; set; }
        public string? Status { get; set; }
        public int ClothesNumber { get; set; }
    }

    public class PlayerInTournamentFVM
    {
        public int Id { get; set; }
        public string? Status { get; set; }
        public int? ClothesNumber { get; set; }
        public int? TeamInTournamentId { get; set; }
        public int? PlayerInTeamId { get; set; }

        public virtual PlayerInTeam? PlayerInTeam { get; set; }
        public virtual TeamInTournament? TeamInTournament { get; set; }
        public virtual ICollection<MatchDetail>? MatchDetails { get; set; }
    }
    public class PlayerInTournamentLV
    {
        public List<PlayerInTournamentVM> PlayerInTournaments { get; set; } = new List<PlayerInTournamentVM>();
        public int CountList { get; set; }
        public int CurrentPage { get; set; }
        public int Size { get; set; }
    }

    public class PlayerInTournamentLFV
    {
        public List<PlayerInTournamentFVM> PlayerInTournaments { get; set; } = new List<PlayerInTournamentFVM>();
        public int CountList { get; set; }
    }
}
