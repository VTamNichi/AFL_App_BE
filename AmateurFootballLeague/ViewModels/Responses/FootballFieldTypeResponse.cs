namespace AmateurFootballLeague.ViewModels.Responses
{
    public class FootballFieldTypeVM
    {
        public int Id { get; set; }
        public string? FootballFieldTypeName { get; set; }
        public string? Description { get; set; }
    }
    public class FootballFieldTypeListVM
    {
        public List<FootballFieldTypeVM> FootballFieldTypes { get; set; } = new List<FootballFieldTypeVM>();
        public int CurrentPage { get; set; }
        public int Size { get; set; }
    }
}
