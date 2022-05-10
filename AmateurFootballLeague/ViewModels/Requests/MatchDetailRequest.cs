using System.ComponentModel.DataAnnotations;

namespace AmateurFootballLeague.ViewModels.Requests
{
    public class MatchDetailCM
    {
        public int MatchScore { get; set; }
        public int YellowCardNumber { get; set; }
        public int RedCardNumber { get; set; }
        [Required]
        public int MatchId { get; set; }
        [Required]
        public int PlayerInTeamId { get; set; }
    }

    public class MatchDetailUM
    {
        public int Id { get; set; }
        public int MatchScore { get; set; }
        public int YellowCardNumber { get; set; }
        public int RedCardNumber { get; set; }
        public int MatchId { get; set; }
        public int PlayerInTeamId { get; set; }
    }

}
