using System.ComponentModel.DataAnnotations;

namespace AmateurFootballLeague.ViewModels.Requests
{
    public class PlayerInTeamCM
    {
        [Required]
        public int TeamId { get; set; }
        [Required]
        public int FootballPlayerId { get; set; }
    }
}
