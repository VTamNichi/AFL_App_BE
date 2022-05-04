namespace AmateurFootballLeague.Models
{
    public class User
    {
        public int ID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Username { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Avatar { get; set; }
        public string? Bio { get; set; }
        public string? StatusBan { get; set; }
        public DateTime? DateBan { get; set; }
        public DateTime? DateUnban { get; set; }
        public int? FlagReportComment { get; set; }
        public int? FlagReportTeam { get; set; }
        public int? FlagReportTournament { get; set; }
        public string? IdentityCard { get; set; }
        public string? PhoneBusiness { get; set; }
        public string? NameBusiness { get; set; }
        public string? TINBusiness { get; set; }
        public bool? Status { get; set; }
        public DateTime? DateCreate { get; set; }
        public DateTime? DateUpdate { get; set; }
        public DateTime? DateDelete { get; set; }
        public Role Role { get; set; }
        public int RoleID { get; set; }
    }
}