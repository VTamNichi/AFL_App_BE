using System.ComponentModel.DataAnnotations;

namespace AmateurFootballLeague.ViewModels.Requests
{
    public enum TypeRegisterOrRecruit
    {
        Register,
        Recruit
    }
    public class EmailForm
    {   
        [Required, EmailAddress]
        public string ToEmail { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Message { get; set; }
    }

    public class SendMailRegisterOrRecruitCM
    {
        public int? TournamentId { get; set; }

        [Required]
        public int FootballPlayerId { get; set; }

        [Required]
        public int TeamId { get; set; }

        [Required]
        public TypeRegisterOrRecruit Type { get; set; }
        public bool Status { get; set; }

    }

    public class SendMailAcceptTeamToTournament
    {
        [Required]
        public int TournamentId { get; set; }

        [Required]
        public int TeamId { get; set; }

        public bool Status { get; set; }

        public string? Reason { get; set; }
    }
}
