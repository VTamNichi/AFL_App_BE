using AmateurFootballLeague.Models;

namespace AmateurFootballLeague.ViewModels.Responses
{
    public class CommentVM
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateDelete { get; set; }
        public DateTime DateUpdate { get; set; }
        public string Status { get; set; }
        public int TeamId { get; set; }
        public int TournamentId { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
    }


    public class CommentLV
    {
        public List<CommentVM> Comments { get; set; } = new List<CommentVM>();
        public int CountList { get; set; }
        public int CurrentPage { get; set; }
        public int Size { get; set; }
    }

}
