using System.ComponentModel.DataAnnotations;

namespace AmateurFootballLeague.ViewModels.Requests
{
    public enum UserFieldEnum
    {
        Id,
        Email,
        Username,
        Gender,
        Phone,
        DateOfBirth,
        DateCreate,
    }
    public enum UserSearchType
    {
        Id,
        Email,
    }
    public enum UserGenderEnum
    {
        Male,
        Female,
        Other,
    }
    public class UserCM
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(64)]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(32)]
        public string Password { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public string Username { get; set; }

        [Required(AllowEmptyStrings = false)]
        public UserGenderEnum Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public IFormFile? Avatar { get; set; }
        public string? Bio { get; set; }
        public string? IdentityCard { get; set; }
        public string? PhoneBusiness { get; set; }
        public string? NameBusiness { get; set; }
        public string? TINBusiness { get; set; }

        [Required(AllowEmptyStrings = false)]
        public int RoleId { get; set; }
    }

    public class UserUM
    {
        [Required(AllowEmptyStrings = false)]
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Username { get; set; }
        public UserGenderEnum? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public IFormFile? Avatar { get; set; }
        public string? Bio { get; set; }
        public string? IdentityCard { get; set; }
        public string? PhoneBusiness { get; set; }
        public string? NameBusiness { get; set; }
        public string? Tinbusiness { get; set; }
        public int? RoleId { get; set; }
    }

    public class UserLEPM
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class UserLM
    {
        public string TokenId { get; set; }
        public int RoleId { get; set; }
    }
    public class UserLOM
    {
        public string Token { get; set; }
        public string Email { get; set; }
    }
}
