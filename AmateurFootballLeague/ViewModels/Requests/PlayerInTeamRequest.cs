using System.ComponentModel.DataAnnotations;

namespace AmateurFootballLeague.ViewModels.Requests
{
    public class PlayerInTeamCM
    {
        public string? Status { get; set; }
        [Required]
        public int TeamId { get; set; }
        [Required]
        public int FootballPlayerId { get; set; }
    }
}
