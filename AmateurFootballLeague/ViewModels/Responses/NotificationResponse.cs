namespace AmateurFootballLeague.ViewModels.Responses
{
    public class NotificationVM
    {
        public int Id { get; set; }
        public string? Content { get; set; }
        public string? DateCreate { get; set; }
        public bool IsSeen { get; set; }
        public bool IsActive { get; set; }
        public int UserId { get; set; }
        public int TournamentId { get; set; }
        public int TeamId { get; set; }
    }

    public class NotificationListVM
    {
        public List<NotificationVM> Notifications { get; set; } = new List<NotificationVM>();
        public int CountList { get; set; }
        public int CountUnRead { get; set; }
        public int CountNew { get; set; }
        public int CurrentPage { get; set; }
        public int Size { get; set; }
    }
}
