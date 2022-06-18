using System.ComponentModel.DataAnnotations;

namespace AmateurFootballLeague.ViewModels.Requests
{
    public enum FootballPlayerGenderEnum
    {
        Male,
        Female,
    }
    public enum FootballPlayerFieldEnum
    {
        Id,
        PlayerName,
        Position,
        DateOfBirth,
    }
    public class FootballPlayerCM
    {
        [Required(AllowEmptyStrings = false)]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public string? PlayerName { get; set; }

        public IFormFile? PlayerAvatar { get; set; }

        public string? Position { get; set; }
        public string? Description { get; set; }
    }
    public class FootballPlayerUM
    {
        [Required(AllowEmptyStrings = false)]
        public int Id { get; set; }

        public string? PlayerName { get; set; }

        public IFormFile? PlayerAvatar { get; set; }

        public string? Position { get; set; }
        public string? Description { get; set; }
    }
}
