using System.ComponentModel.DataAnnotations;

namespace AmateurFootballLeague.ViewModels.Requests
{
    public enum RoleFieldEnum
    {
        Id,
        RoleName
    }
    // role create model
    public class RoleCM
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(16)]
        public string? RoleName { get; set; }
    }
}
