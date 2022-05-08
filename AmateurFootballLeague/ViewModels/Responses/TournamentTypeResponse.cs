namespace AmateurFootballLeague.ViewModels.Responses
{
    public class TournamentTypeVM
    {
        public int Id { get; set; }
        public string TournamentTypeName { get; set; }
        public string Description { get; set; }
    }
    public class TournamentTypeListVM
    {
        public List<TournamentTypeVM> TournamentTypes { get; set; } = new List<TournamentTypeVM>();
        public int CurrentPage { get; set; }
        public int Size { get; set; }
    }
}
