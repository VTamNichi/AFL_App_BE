using System.ComponentModel.DataAnnotations;

namespace AmateurFootballLeague.ViewModels.Requests
{
    public class TeamInMatchCM
    {
        [Required]
        public int TeamScore { get; set; }
        public int? YellowCardNumber { get; set; }
        public int? RedCardNumber { get; set; }
        [Required]
        public int TeamId { get; set; }
        [Required]
        public int MatchId { get; set; }
        public string? Result { get; set; }
        public string? NextTeam { get; set; }
        public string? TeamName { get; set; }
    }

    public class TeamInMatchUM
    {
        public int Id { get; set; }
        public int TeamScore { get; set; }
        public int? YellowCardNumber { get; set; }
        public int? RedCardNumber { get; set; }
        public int TeamId { get; set; }
        public int MatchId { get; set; }
        public string? Result { get; set; }
        public string? NextTeam { get; set; }
        public string? TeamName { get; set; }
    }
}
