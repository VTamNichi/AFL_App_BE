namespace AmateurFootballLeague.ViewModels.Responses
{
    public class MatchVM
    {
        public int Id { get; set; }
        public string MatchDate { get; set; }
        public string Status { get; set; }
    }
    public class MatchListVM
    {
        public List<MatchVM> Matchs { get; set; } = new List<MatchVM>();
        public int CurrentPage { get; set; }
        public int Size { get; set; }
    }
}
