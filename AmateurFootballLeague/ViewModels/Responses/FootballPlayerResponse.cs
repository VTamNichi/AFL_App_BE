namespace AmateurFootballLeague.ViewModels.Responses
{
    public class FootballPlayerVM
    {
        public int Id { get; set; }
        public string PlayerName { get; set; }

        public string PlayerAvatar { get; set; }

        public string Position { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public string DateCreate { get; set; }
        public string DateUpdate { get; set; }
        public string DateDelete { get; set; }
    }
    public class FootballPlayerListVM
    {
        public List<FootballPlayerVM> FootballPlayers { get; set; } = new List<FootballPlayerVM>();
        public int CountList { get; set; }
        public int CurrentPage { get; set; }
        public int Size { get; set; }
    }
}
