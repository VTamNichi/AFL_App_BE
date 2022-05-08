using System.ComponentModel.DataAnnotations;

namespace AmateurFootballLeague.ViewModels.Requests
{
    public enum FootballPlayerGenderEnum
    {
        Male,
        Female,
        Other,
    }
    public enum FootballPlayerFieldEnum
    {
        Id,
        Email,
        PlayerName,
        DateOfBirth,
        ClothersNumber,
    }
    public class FootballPlayerCM
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(64)]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public string PlayerName { get; set; }

        [Required(AllowEmptyStrings = false)]
        public FootballPlayerGenderEnum Gender { get; set; } = FootballPlayerGenderEnum.Male;

        public DateTime? DateOfBirth { get; set; }

        public IFormFile? PlayerAvatar { get; set; }

        public int? ClothersNumber { get; set; }
    }
    public class FootballPlayerUM
    {
        [Required(AllowEmptyStrings = false)]
        public int Id { get; set; }

        public string? Email { get; set; }

        public string? PlayerName { get; set; }

        public FootballPlayerGenderEnum? Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public IFormFile? PlayerAvatar { get; set; }

        public int? ClothersNumber { get; set; }
    }
}
