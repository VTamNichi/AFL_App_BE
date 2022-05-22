namespace AmateurFootballLeague.ViewModels.Responses
{
    public class UserVM
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Avatar { get; set; }
        public string Bio { get; set; }
        public string StatusBan { get; set; }
        public DateTime DateBan { get; set; }
        public DateTime DateUnban { get; set; }
        public int FlagReportComment { get; set; }
        public int FlagReportTeam { get; set; }
        public int FlagReportTournament { get; set; }
        public string IdentityCard { get; set; }
        public string PhoneBusiness { get; set; }
        public string NameBusiness { get; set; }
        public string Tinbusiness { get; set; }
        public bool Status { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateUpdate { get; set; }
        public DateTime DateDelete { get; set; }
        public int RoleId { get; set; }
    }

    public class UserListVM
    {
        public List<UserVM> Users { get; set; } = new List<UserVM>();
        public int CountList { get; set; }
        public int CurrentPage { get; set; }
        public int Size { get; set; }
    }
    public class UserLVM
    {
        public UserVM UserVM { get; set; }
        public string AccessToken { get; set; }
    }
}
