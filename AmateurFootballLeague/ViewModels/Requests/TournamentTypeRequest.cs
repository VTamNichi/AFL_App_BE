using System.ComponentModel.DataAnnotations;

namespace AmateurFootballLeague.ViewModels.Requests
{
    public enum TournamentTypeFieldEnum
    {
        Id,
        TournamentTypeName
    }

    public class TournamentTypeCM
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public string? TournamentTypeName { get; set; }

        [StringLength(256)]
        public string? Description { get; set; }
    }
}
