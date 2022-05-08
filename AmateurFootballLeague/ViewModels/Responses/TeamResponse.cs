namespace AmateurFootballLeague.ViewModels.Responses
{
    public class TeamVM
    {
        public int Id { get; set; }
        public string TeamName { get; set; }
        public string TeamAvatar { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
    }
    public class TeamListVM
    {
        public List<TeamVM> Teams { get; set; } = new List<TeamVM>();
        public int CurrentPage { get; set; }
        public int Size { get; set; }
    }
}
