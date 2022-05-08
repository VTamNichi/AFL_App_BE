namespace AmateurFootballLeague.ViewModels.Responses
{
    public class FootballPlayerVM
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Playername { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PlayerAvatar { get; set; }
        public int ClothersNumber { get; set; }
        public bool Status { get; set; }
    }
    public class FootballPlayerListVM
    {
        public List<FootballPlayerVM> FootballPlayers { get; set; } = new List<FootballPlayerVM>();
        public int CurrentPage { get; set; }
        public int Size { get; set; }
    }
}
