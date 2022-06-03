using System;
using System.Collections.Generic;

namespace AmateurFootballLeague.Models
{
    public partial class Comment
    {
        public int Id { get; set; }
        public string? Content { get; set; }
        public string? Status { get; set; }
        public DateTime? DateCreate { get; set; }
        public DateTime? DateDelete { get; set; }
        public DateTime? DateUpdate { get; set; }
        public int? MatchId { get; set; }
        public int? TeamId { get; set; }
        public int? TournamentId { get; set; }
        public int? UserId { get; set; }

        public virtual Match? Match { get; set; }
        public virtual Team? Team { get; set; }
        public virtual Tournament? Tournament { get; set; }
        public virtual User? User { get; set; }
    }
}
