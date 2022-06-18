using System.ComponentModel.DataAnnotations;

namespace AmateurFootballLeague.ViewModels.Requests
{
    public enum TeamFieldEnum
    {
        Id,
        TeamName,
    }
    public enum TeamGenderEnum
    {
        Male,
        Female,
    }
    public class TeamCM
    {
        [Required(AllowEmptyStrings = false)]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public string? TeamName { get; set; }

        public IFormFile? TeamAvatar { get; set; }

        public string? TeamArea { get; set; }

        public string? TeamPhone { get; set; }
        
        public TeamGenderEnum? TeamGender { get; set; }

        public string? Description { get; set; }
    }
    public class TeamUM
    {
        [Required(AllowEmptyStrings = false)]
        public int Id { get; set; }

        public string? TeamName { get; set; }

        public IFormFile? TeamAvatar { get; set; }

        public string? TeamArea { get; set; }

        public string? TeamPhone { get; set; }

        public TeamGenderEnum? TeamGender { get; set; }

        public string? Description { get; set; }
    }
}
