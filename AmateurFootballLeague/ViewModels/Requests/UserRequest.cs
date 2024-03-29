﻿using System.ComponentModel.DataAnnotations;

namespace AmateurFootballLeague.ViewModels.Requests
{
    public enum UserFieldEnum
    {
        Id,
        Email,
        Username,
        Gender,
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
    }
    public class UserCM
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(64)]
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
        public DateTime? DateIssuance { get; set; }
        public string? PhoneBusiness { get; set; }
        public string? NameBusiness { get; set; }
        public string? TINBusiness { get; set; }

        [Required(AllowEmptyStrings = false)]
        public int RoleId { get; set; }
    }

    public class UserGGCM
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(64)]
        public string? Email { get; set; }
        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public string? Username { get; set; }
        public string? Avatar { get; set; }

        [Required(AllowEmptyStrings = false)]
        public UserGenderEnum Gender { get; set; }
        public string? Phone { get; set; }
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
        public int? FlagReportFootballPlayer { get; set; }
        public int? FlagReportTeam { get; set; }
        public int? FlagReportTournament { get; set; }
        public string? IdentityCard { get; set; }
        public DateTime? DateIssuance { get; set; }
        public string? PhoneBusiness { get; set; }
        public string? NameBusiness { get; set; }
        public string? Tinbusiness { get; set; }
        public int? CountBlock { get; set; }
        public int? RoleId { get; set; }
    }

    public class UserLEPM
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
    public class UserLM
    {
        public string? TokenId { get; set; }
    }
    public class UserLOM
    {
        public string? Token { get; set; }
        public string? Email { get; set; }
    }
    public class UserCPM
    {
        [Required(AllowEmptyStrings = false)]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public string? CurrentPassword { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public string? NewPassword { get; set; }
    }

    public class UserRPM
    {

        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public string? Email { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public string? NewPassword { get; set; }
    }
}
