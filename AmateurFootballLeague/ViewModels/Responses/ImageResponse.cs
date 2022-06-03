namespace AmateurFootballLeague.ViewModels.Responses
{
    public class ImageVM
    {
        public int Id { get; set; }
        public string ImageURL { get; set; }
        public string Status { get; set; }
        public string DateCreate { get; set; }
        public string DateUpdate { get; set; }
        public string DateDelete { get; set; }
        public int TournamentId { get; set; }
    }
    public class ImageListVM
    {
        public List<ImageVM> Images { get; set; } = new List<ImageVM>();
        public int CountList { get; set; }
        public int CurrentPage { get; set; }
        public int Size { get; set; }
    }
}
