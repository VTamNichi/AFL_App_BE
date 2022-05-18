using System.ComponentModel.DataAnnotations;

namespace AmateurFootballLeague.ViewModels.Requests
{
    public enum TournamentModeEnum
    {
        PUBLIC,
        PRIVATE,
    }
    public enum TournamentTypeEnum
    {
        KnockoutStage,
        CircleStage,
        GroupStage
    }
    public enum TournamentFootballFieldTypeEnum
    {
        Field5,
        Field7,
        Field11
    }
    public enum TournamentFieldEnum
    {
        TournamentName,
        Mode,
        DateCreate,
    }

    public enum TournamentGenderEnum
    {
        Male,
        Female,
    }
    public class TournamentCM
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public string TournamentName { get; set; }

        [Required(AllowEmptyStrings = false)]
        public TournamentModeEnum Mode { get; set; } = TournamentModeEnum.PUBLIC;

        public string? TournamentPhone { get; set; }
        
        public TournamentGenderEnum? TournamentGender { get; set; }


        public DateTime? RegisterEndDate { get; set; }

        public DateTime? TournamentStartDate { get; set; }

        public DateTime? TournamentEndDate { get; set; }

        public string? FootballFieldAddress { get; set; }
        
        public IFormFile? TournamentAvatar { get; set; }
        
        public string? Description { get; set; }

        [Required(AllowEmptyStrings = false)]
        public int MatchMinutes { get; set; }

        [Required(AllowEmptyStrings = false)]
        public int FootballTeamNumber { get; set; }

        [Required(AllowEmptyStrings = false)]
        public int FootballPlayerMaxNumber { get; set; }

        [Required(AllowEmptyStrings = false)]
        public int UserId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public TournamentTypeEnum TournamentTypeEnum { get; set; }

        [Required(AllowEmptyStrings = false)]
        public TournamentFootballFieldTypeEnum TournamentFootballFieldTypeEnum { get; set; }
    }
    public class TournamentUM
    {
        [Required(AllowEmptyStrings = false)]
        public int Id { get; set; }

        public string? TournamentName { get; set; }

        public TournamentModeEnum? Mode { get; set; }

        public string? TournamentPhone { get; set; }
        public TournamentGenderEnum? TournamentGender { get; set; }

        public DateTime? RegisterEndDate { get; set; }

        public DateTime? TournamentStartDate { get; set; }

        public DateTime? TournamentEndDate { get; set; }

        public string? FootballFieldAddress { get; set; }
        
        public IFormFile? TournamentAvatar { get; set; }
        
        public string? Description { get; set; }

        public int? MatchMinutes { get; set; }

        public int? FootballTeamNumber { get; set; }

        public int? FootballPlayerMaxNumber { get; set; }

        public TournamentTypeEnum? TournamentTypeEnum { get; set; }

        public TournamentFootballFieldTypeEnum? TournamentFootballFieldTypeEnum { get; set; }
    }
}
