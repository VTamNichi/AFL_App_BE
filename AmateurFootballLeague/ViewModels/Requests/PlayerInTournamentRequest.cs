using System.ComponentModel.DataAnnotations;

namespace AmateurFootballLeague.ViewModels.Requests
{
    public class PlayerInTournamentCM
    {
        [Required]
        public int TeamInTournamentId { get; set; }
        [Required]
        public int PlayerInTeamId { get; set; }

        public string? Status { get; set; }
        public int? ClothesNumber { get; set; }
    }
}
