using System.ComponentModel.DataAnnotations;

namespace AmateurFootballLeague.ViewModels.Requests
{
    public enum FootballFieldTypeEnum
    {
        Id,
        FootballFieldTypeName
    }

    public class FootballFieldTypeCM
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public string FootballFieldTypeName { get; set; }

        [StringLength(256)]
        public string? Description { get; set; }
    }
}
