namespace AmateurFootballLeague.ViewModels.Responses
{
    // role view model
    public class RoleVM
    {
        public int Id { get; set; }
        public string? RoleName { get; set; }
    }
    public class RoleListVM
    {
        public List<RoleVM> Roles { get; set;} = new List<RoleVM>();
        public int CurrentPage { get; set; }
        public int Size { get; set; }
    }
}
